using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.PronunciationAssessment;
using Microsoft.CognitiveServices.Speech.Translation;
using NAudio.Wave;

namespace Allen.Application;

[RegisterService(typeof(IAzureSpeechsService))]
public class AzureSpeechsService(IAppConfiguration _configuration) : IAzureSpeechsService
{

    public async Task<List<TranscribeResponseModel>> Transcribe(TranscribeRequestModel model)
    {
        var tempMp3Path = Path.GetTempFileName();
        using (var stream = new FileStream(tempMp3Path, FileMode.Create))
        {
            await model.File.CopyToAsync(stream);
        }

        var azureSpeechSetting = _configuration.GetAzureSpeechSetting();
        var speechKey = azureSpeechSetting.Key;
        var speechRegion = azureSpeechSetting.Region;

        var speechTranslationConfig = SpeechTranslationConfig.FromSubscription(speechKey, speechRegion);
        speechTranslationConfig.SpeechRecognitionLanguage = "en-US";
        speechTranslationConfig.AddTargetLanguage("vi");

        // Bật detailed result để có word-level timestamps
        speechTranslationConfig.SetProperty("SpeechServiceResponse_RequestDetailedResultTrue", "true");
        speechTranslationConfig.SetProperty("SpeechServiceResponse_ProfanityOption", "Raw");


        var resultList = new List<TranscribeResponseModel>();
        int index = 1;

        var wavPath = ConvertMp3ToWav(tempMp3Path);

        try
        {
            using var audioConfig = AudioConfig.FromWavFileInput(wavPath);
            using var recognizer = new TranslationRecognizer(speechTranslationConfig, audioConfig);

            var tcs = new TaskCompletionSource<bool>();

            recognizer.Recognized += (s, e) =>
            {
                if (e.Result.Reason == ResultReason.TranslatedSpeech)
                {
                    var enText = e.Result.Text;
                    var viText = e.Result.Translations.ContainsKey("vi") ? e.Result.Translations["vi"] : "";

                    resultList.Add(new TranscribeResponseModel
                    {
                        OrderIndex = index++,
                        StartTime = e.Result.OffsetInTicks / TimeSpan.TicksPerSecond,
                        EndTime = (e.Result.OffsetInTicks + e.Result.Duration.Ticks) / TimeSpan.TicksPerSecond,
                        ContentEN = enText,
                        ContentVN = viText,
                        IPA = null
                    });
                }
            };

            recognizer.SessionStopped += (s, e) => tcs.TrySetResult(true);
            recognizer.Canceled += (s, e) => tcs.TrySetResult(true);

            await recognizer.StartContinuousRecognitionAsync();
            await tcs.Task;
            await recognizer.StopContinuousRecognitionAsync();

        }
        finally
        {
            if (File.Exists(tempMp3Path)) File.Delete(tempMp3Path);
            if (File.Exists(wavPath)) File.Delete(wavPath);
        }
        return resultList;
    }
    public async Task TranscribeStreamAsync(TranscribeRequestModel model, StreamWriter writer)
    {
        var tempMp3Path = Path.GetTempFileName();
        await using (var stream = new FileStream(tempMp3Path, FileMode.Create))
        {
            await model.File.CopyToAsync(stream);
        }

        var azureSpeechSetting = _configuration.GetAzureSpeechSetting();
        var speechConfig = SpeechTranslationConfig.FromSubscription(
            azureSpeechSetting.Key, azureSpeechSetting.Region);

        speechConfig.SpeechRecognitionLanguage = "en-US";
        speechConfig.AddTargetLanguage("vi");

        var wavPath = ConvertMp3ToWav(tempMp3Path);
        // ✅ Khai báo options để không escape Unicode
        var jsonOptions = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = false
        };
        try
        {
            using var audioConfig = AudioConfig.FromWavFileInput(wavPath);
            using var recognizer = new TranslationRecognizer(speechConfig, audioConfig);

            var tcs = new TaskCompletionSource<bool>();
            int index = 1;
            recognizer.Recognized += async (s, e) =>
            {
                if (e.Result.Reason == ResultReason.TranslatedSpeech)
                {
                    var obj = new TranscribeResponseModel
                    {
                        OrderIndex = index++,
                        StartTime = e.Result.OffsetInTicks / TimeSpan.TicksPerSecond,
                        EndTime = (e.Result.OffsetInTicks + e.Result.Duration.Ticks) / TimeSpan.TicksPerSecond,
                        ContentEN = e.Result.Text,
                        ContentVN = e.Result.Translations.ContainsKey("vi") ? e.Result.Translations["vi"] : "",
                        IPA = null
                    };

                    var json = JsonSerializer.Serialize(obj, jsonOptions);
                    await writer.WriteLineAsync(json);
                    await writer.FlushAsync();
                }
            };

            recognizer.SessionStopped += (s, e) => tcs.TrySetResult(true);
            recognizer.Canceled += (s, e) => tcs.TrySetResult(true);

            await recognizer.StartContinuousRecognitionAsync();
            await tcs.Task;
            await recognizer.StopContinuousRecognitionAsync();
        }
        finally
        {
            if (File.Exists(tempMp3Path)) File.Delete(tempMp3Path);
            if (File.Exists(wavPath)) File.Delete(wavPath);
        }
    }

	public async Task<PronunciationAnalysisResultModel> AnalyzePronunciationAsync(PronunciationModel request)
	{
		try
		{
			// ===========================
			// GHI FILE TẠM MP3 AN TOÀN
			// ===========================
			var tempMp3Path = Path.ChangeExtension(Path.GetTempFileName(), ".mp3");
			using (var fileStream = File.Create(tempMp3Path))
			{
				await request.AudioFile.CopyToAsync(fileStream);
			}

			// ===========================
			// CHUYỂN MP3 -> WAV (CHUẨN 16kHz)
			// ===========================
			var wavPath = ConvertMp3ToWav(tempMp3Path);

			// ===========================
			// GỌI AZURE SPEECH
			// ===========================
			var result = await AnalyzeWithAzureSpeech(wavPath, request.ReferenceText, request.Accent ?? "US");

			// ===========================
			// DỌN FILE TẠM
			// ===========================
			System.IO.File.Delete(tempMp3Path);
			System.IO.File.Delete(wavPath);

			return result;
		}
		catch (Exception ex)
		{
			// Giữ nguyên khối catch
			throw new Exception(ex.Message);
		}
	}

	private async Task<PronunciationAnalysisResultModel> AnalyzeWithAzureSpeech(string audioFilePath, string referenceText, string accent)
	{
		// ===========================
		// CẤU HÌNH AZURE SPEECH
		// ===========================
		var azureSpeechSetting = _configuration.GetAzureSpeechSetting();
		var speechKey = azureSpeechSetting.Key;
		var speechRegion = azureSpeechSetting.Region;

		// Có thể reuse _speechConfig nếu class này là singleton
		var config = SpeechConfig.FromSubscription(speechKey, speechRegion);
		config.SpeechRecognitionLanguage = "en-US";

		// ===========================
		// CẤU HÌNH CHẤM PHÁT ÂM
		// ===========================
		var pronunciationConfig = new PronunciationAssessmentConfig(
			referenceText,
			GradingSystem.HundredMark,
			Granularity.Word);   // nhanh hơn Phoneme rất nhiều
								 //Granularity.Phoneme);

		using var audioInput = AudioConfig.FromWavFileInput(audioFilePath);
		using var recognizer = new SpeechRecognizer(config, audioInput);

		pronunciationConfig.ApplyTo(recognizer);

		// ===========================
		// THỰC THI RECOGNITION
		// ===========================
		var result = await recognizer.RecognizeOnceAsync();

		if (result.Reason != ResultReason.RecognizedSpeech)
			throw new Exception($"Speech recognition failed: {result.Reason}");

		// ===========================
		// LẤY JSON KẾT QUẢ
		// ===========================
		string jsonResult;
		try
		{
			jsonResult = result.Properties.GetProperty(PropertyId.SpeechServiceResponse_JsonResult);
		}
		catch (KeyNotFoundException)
		{
			throw new Exception("Pronunciation detail JSON not found in Azure response.");
		}

		// ===========================
		// PHÂN TÍCH KẾT QUẢ
		// ===========================
		var pronunciationResult = PronunciationAssessmentResult.FromResult(result);
		var pronBand = ConvertAzurePronunciationToIelts(pronunciationResult.PronunciationScore);
		return new PronunciationAnalysisResultModel
		{
			TranscribedText = result.Text,
			ReferenceText = referenceText,
			//OverallScore = CalculateOverallScore(pronunciationResult),
			AccuracyScore = pronunciationResult.AccuracyScore,
			FluencyScore = pronunciationResult.FluencyScore,
			CompletenessScore = pronunciationResult.CompletenessScore,
			PronDetailScore = pronBand,
			PronunciationFeedback = GeneratePronunciationFeedback(pronBand),
			WordAnalysis = ExtractWordAnalysis(jsonResult),
		};
	}

	public static string ConvertMp3ToWav(string mp3Path)
	{
		if (!File.Exists(mp3Path))
			throw new FileNotFoundException("Input file not found.", mp3Path);

		// Tạo file WAV tạm
		var wavPath = Path.ChangeExtension(Path.GetTempFileName(), ".wav");

		using (var reader = new MediaFoundationReader(mp3Path))
		{
			var newFormat = new WaveFormat(16000, 16, 1);

			// Chỉ resample khi cần thiết
			if (reader.WaveFormat.SampleRate != 16000 || reader.WaveFormat.Channels != 1)
			{
				using (var resampler = new MediaFoundationResampler(reader, newFormat))
				{
					resampler.ResamplerQuality = 60; // chất lượng vừa đủ, 0-60
					WaveFileWriter.CreateWaveFile(wavPath, resampler);
				}
			}
			else
			{
				WaveFileWriter.CreateWaveFile(wavPath, reader);
			}
		}

		if (!File.Exists(wavPath))
			throw new Exception("Convert MP3 to WAV failed.");

		return wavPath;
	}

	//private double CalculateOverallScore(PronunciationAssessmentResult result)
	//{
	//	return Math.Round(
	//		(result.AccuracyScore * 0.5) +
	//		(result.FluencyScore * 0.3) +
	//		(result.CompletenessScore * 0.2), 2
	//	);
	//}

	private List<WordAnalysisResult> ExtractWordAnalysis(string jsonResult)
	{
		var wordAnalysis = new List<WordAnalysisResult>();

		using var doc = JsonDocument.Parse(jsonResult);
		var root = doc.RootElement;

		if (root.TryGetProperty("NBest", out var nbestArray) && nbestArray.GetArrayLength() > 0)
		{
			var firstNBest = nbestArray[0];
			if (firstNBest.TryGetProperty("Words", out var wordsElement))
			{
				foreach (var wordElement in wordsElement.EnumerateArray())
				{
					var assessment = wordElement.GetProperty("PronunciationAssessment");

					var word = new WordAnalysisResult
					{
						Word = wordElement.GetProperty("Word").GetString() ?? "",
						AccuracyScore = assessment.GetProperty("AccuracyScore").GetDouble(),
						ErrorType = assessment.GetProperty("ErrorType").GetString() ?? "",
						//PhonemeAnalysis = new List<PhonemeAnalysisModel>()
					};

					//if (wordElement.TryGetProperty("Phonemes", out var phonemesElement))
					//{
					//    foreach (var phonemeElement in phonemesElement.EnumerateArray())
					//    {
					//        var phonemeAssessment = phonemeElement.GetProperty("PronunciationAssessment");

					//        word.PhonemeAnalysis.Add(new PhonemeAnalysisModel
					//        {
					//            Phoneme = phonemeElement.GetProperty("Phoneme").GetString() ?? "",
					//            AccuracyScore = phonemeAssessment.GetProperty("AccuracyScore").GetDouble()
					//        });
					//    }
					//}

					wordAnalysis.Add(word);
				}
			}
		}

		return wordAnalysis;
	}
	private double ConvertAzurePronunciationToIelts(double pronunciationScore)
	{
		// Bước 1: Chuyển từ thang 100 → 9
		var ieltsBand = pronunciationScore / 100 * 9;

		// Bước 2: Làm tròn theo quy tắc IELTS
		var floor = Math.Floor(ieltsBand);
		var fraction = ieltsBand - floor;

		if (fraction < 0.25)
			return floor;
		else if (fraction < 0.75)
			return floor + 0.5;
		else
			return floor + 1.0;
	}
	private string GeneratePronunciationFeedback(double band)
	{
		if (band >= 8.5)
			return "9.0\nYour pronunciation is excellent. You use a wide range of features with precision and ease. Intonation and stress are natural, and every word is clear and effortless to understand.";

		if (band >= 8.0)
			return "8.0\nYour pronunciation is very good. You are easy to understand throughout. Minor lapses in stress or intonation may occur but do not affect comprehension.";

		if (band >= 7.0)
			return "7.0\nYour pronunciation is good. Most sounds are clear and natural, though occasional mispronunciations or misplaced stress may appear.";

		if (band >= 6.0)
			return "6.0\nYour pronunciation is generally clear and understandable, but some sounds or words are occasionally mispronounced. Listeners may need minor effort to understand.";

		if (band >= 5.0)
			return "5.0\nYour pronunciation is average. It is understandable but not always accurate. Focusing on the rhythm and stress patterns of English could prove beneficial.";

		if (band >= 4.0)
			return "4.0\nYour pronunciation is often unclear and requires effort from the listener. Mispronunciations of common words and sounds are frequent.";

		if (band >= 3.0)
			return "3.0\nYour pronunciation makes understanding difficult. Many words are pronounced inaccurately or distorted. Practice basic English sounds and word endings.";

		return "2.0\nYour pronunciation is poor and causes serious difficulty for communication. You need to work on individual sounds, stress, and basic rhythm.";
	}
}
