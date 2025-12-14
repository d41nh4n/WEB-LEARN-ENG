namespace Allen.Application;

public interface IAzureSpeechsService
{
    Task<PronunciationAnalysisResultModel> AnalyzePronunciationAsync(PronunciationModel model);
    Task<List<TranscribeResponseModel>> Transcribe(TranscribeRequestModel model);
    Task TranscribeStreamAsync(TranscribeRequestModel model, StreamWriter writer);
}