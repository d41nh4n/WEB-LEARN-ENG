using System.Text.RegularExpressions;

namespace Allen.Application;

[RegisterService(typeof(ISpeakingsService))]
internal sealed class SpeakingsService(
    ISpeakingsRepository _repository,
	IUserPointService _userPointService,
	IBlobStorageService _blobStorageService,
    IAzureSpeechsService _azureSpeechsService,
    IGeminiService _geminiService,
	IUnitOfWork _unitOfWork,
	IMapper _mapper) : ISpeakingsService
{
    public async Task<SpeakingModel> GetByLearningUnitIdAsync(Guid learningUnitId)
        => await _repository.GetByLearningUnitIdAsync(learningUnitId);
	public async Task<List<SpeakingForIeltsModel>> GetByLearningUnitIdForIeltsAsync(Guid learningUnitId, GetByLearningUnitIdForIeltsQuery query)
        => await _repository.GetByLearningUnitIdForIeltsAsync(learningUnitId, query);
	public async Task<OperationResult> CreateLearningAsync(CreateSpeakingModel model)
    {
        var learningUnitEntity = _mapper.Map<LearningUnitEntity>(model.LearningUnit);
        var speakingEntity = _mapper.Map<SpeakingEntity>(model);
        speakingEntity.LearningUnit!.SkillType = SkillType.Speaking;
		speakingEntity.LearningUnit!.LearningUnitType = LearningUnitType.Academy;
        speakingEntity.LearningUnit!.LearningUnitStatusType = LearningUnitStatusType.Draft;
        try
        {
            await _unitOfWork.ExecuteWithTransactionAsync(async () =>
            {
                await _unitOfWork.Repository<SpeakingEntity>().AddAsync(speakingEntity);
                await _unitOfWork.SaveChangesAsync();
            });
            return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(SpeakingEntity)));
        }
        catch
        {
            await _blobStorageService.DeleteFileByUrlAsync(speakingEntity.Media!.SourceUrl);
            return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(SpeakingEntity)));
        }
    }
	public async Task<OperationResult> CreateIeltsAsync(CreateOrUpdateSpeakingIeltsModel model)
	{
		if (!await _unitOfWork.Repository<LearningUnitEntity>().CheckExistAsync(x => x.Id == model.LearningUnitId && x.SkillType == SkillType.Speaking && x.LearningUnitType == LearningUnitType.Ielts))
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(LearningUnitEntity), model.LearningUnitId));

		var existingSections = await _unitOfWork.Repository<SpeakingEntity>()
			.GetListByConditionAsync(x => x.LearningUnitId == model.LearningUnitId);

		if (!existingSections.Any() && model.SectionIndex != 1)
		{
			return OperationResult.Failure("The first Speaking section must have SectionIndex = 1.");
		}

		var maxIndex = existingSections.Any() ? existingSections.Max(x => x.SectionIndex) : 0;

		if (model.SectionIndex != maxIndex + 1)
		{
			return OperationResult.Failure($"Invalid SectionIndex. The next valid index should be {maxIndex + 1}.");
		}

		if (existingSections.Any(x => x.SectionIndex == model.SectionIndex))
		{
			return OperationResult.Failure($"SectionIndex {model.SectionIndex} already exists.");
		}

		var entity = _mapper.Map<SpeakingEntity>(model);
		await _unitOfWork.Repository<SpeakingEntity>().AddAsync(entity);

		if (!await _unitOfWork.SaveChangesAsync())
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(SpeakingEntity)));

		return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.CreatedSuccess, nameof(SpeakingEntity)), entity.Id);
	}
	
	public async Task<OperationResult> SubmitAsync(SubmitSpeakingModel model)
    {
        var transcript = await _repository.GetTranscriptByIdAsync(model.TranscriptId);

        var (highlighted, isCorrect, accuracy) = CompareSentencesFlexible(
            transcript.ContentEN ?? "",
            model.Text
        );

        var data = new
        {
            Highlighted = highlighted,
            Accuracy = accuracy
        };

        if (isCorrect)
        {
            return OperationResult.SuccessResult("IsCorrect", data);
        }
        else
        {
            return OperationResult.Failure("NotCorrect", data);
        }
    }
	public async Task<OperationResult> SubmitIeltsAsync(Guid userId, SubmitSpeakingIeltsModel model)
	{
        var pronunciationModel = _mapper.Map<PronunciationModel>(model);
		var analyzeResults = await _azureSpeechsService.AnalyzePronunciationAsync(pronunciationModel);
        var geminiRequest = new GeminiRequest
        {
            Prompt = model.ReferenceText,
            Description = model.Question
		};
        var aiFeedback = await _geminiService.AIAgentSubmitSpeakingAsync(geminiRequest);

        var userPointResult = await _userPointService.UsePointsAsync(userId, 10);

		analyzeResults.OverallBand =StringHelper.RoundToNearestHalf(
		        ((aiFeedback?.FluencyCoherence?.Score ?? 0)
		          + (aiFeedback?.LexicalResource?.Score ?? 0)
		          + (aiFeedback?.GrammarAccuracy?.Score ?? 0)
		          + (analyzeResults?.PronDetailScore ?? 0)) / 4.0
	            );
		return OperationResult.SuccessResult("Submitted successfully", new
        {
            AnalyzeResults = analyzeResults,
            AIFeedback = aiFeedback
        });
	}
	public async Task<OperationResult> UpdateLearningAsync(Guid id, UpdateSpeakingModel model)
    {
        var speakingEntity = await _unitOfWork.Repository<SpeakingEntity>().GetByIdAsync(id);
        if (speakingEntity == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(SpeakingEntity), id));

        _mapper.Map(model, speakingEntity);

        var validationError = await ValidateForeignKeys(id, model);
        if (validationError != null)
            return validationError;

        try
        {
            await _unitOfWork.ExecuteWithTransactionAsync(async () =>
            {
                foreach (var transcript in speakingEntity.Media!.Transcripts)
                {
                    transcript.MediaId = speakingEntity.MediaId ?? Guid.Empty;
                }
                _unitOfWork.Repository<SpeakingEntity>().UpdateAsync(speakingEntity);
                var result = await _unitOfWork.SaveChangesAsync();
                if (!result)
                {
                    throw new Exception(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(User)));
                }
            });
            return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(SpeakingEntity)));
        }
        catch (Exception exception)
        {
            if (model.Media != null)
                await _blobStorageService.DeleteFileByUrlAsync(speakingEntity.Media!.SourceUrl);
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(SpeakingEntity), exception.InnerException?.Message ?? exception.Message));
        }
    }
	public async Task<OperationResult> UpdateIeltsAsync(Guid id, CreateOrUpdateSpeakingIeltsModel model)
	{
		var repository = _unitOfWork.Repository<SpeakingEntity>();
		var entity = await repository.GetByIdAsync(id);

		if (!await _unitOfWork.Repository<LearningUnitEntity>().CheckExistAsync(
			x => x.Id == model.LearningUnitId &&
				 x.SkillType == SkillType.Speaking &&
				 x.LearningUnitType == LearningUnitType.Ielts))
		{
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(LearningUnitEntity), model.LearningUnitId));
		}

		var existingSections = await repository.GetListByConditionAsync(x => x.LearningUnitId == model.LearningUnitId);

		if (!existingSections.Any() && model.SectionIndex != 1)
		{
			return OperationResult.Failure("The first Speaking section must have SectionIndex = 1.");
		}

		if (existingSections.Any(x => x.SectionIndex == model.SectionIndex && x.Id != id))
		{
			return OperationResult.Failure($"SectionIndex {model.SectionIndex} already exists in this LearningUnit.");
		}

		var maxIndex = existingSections.Max(x => x.SectionIndex);
		if (model.SectionIndex > maxIndex + 1)
		{
			return OperationResult.Failure($"Invalid SectionIndex. The next valid index should be {maxIndex + 1}.");
		}

		_mapper.Map(model, entity);

		if (!await _unitOfWork.SaveChangesAsync())
			return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(SpeakingEntity)));

		return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(SpeakingEntity)));
	}

	private async Task<OperationResult?> ValidateForeignKeys(Guid id, UpdateSpeakingModel model)
    {
        if (!await _unitOfWork.Repository<CategoryEntity>().CheckExistAsync(x => x.Id == model.LearningUnit.CategoryId))
        {
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.NotExists, nameof(CategoryEntity), model.LearningUnit.CategoryId!));
        }
        if (!await _unitOfWork.Repository<SpeakingEntity>().CheckExistAsync(x => x.Id == id && x.LearningUnitId == model.LearningUnit.Id && x.MediaId == model.Media.Id))
        {
            return OperationResult.Failure("Không được phép thay đổi LearningUnitId hoặc MediaId của SpeakingEntity.");
        }
        return null;
    }
    private (string Highlighted, bool IsCorrect, double Accuracy) CompareSentencesFlexible(string original, string input, double threshold = 0.85)
    {
        var cleanOriginal = Regex.Replace(original.ToLower(), @"[^\w\s]", "");
        var cleanInput = Regex.Replace(input.ToLower(), @"[^\w\s]", "");

        var originalWords = cleanOriginal.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var inputWords = cleanInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var maxLen = Math.Max(originalWords.Length, inputWords.Length);
        var matches = 0;
        var result = new List<string>();

        for (int i = 0; i < maxLen; i++)
        {
            if (i < originalWords.Length && i < inputWords.Length)
            {
                if (originalWords[i] == inputWords[i])
                {
                    result.Add(originalWords[i]);
                    matches++;
                }
                else
                {
                    result.Add($"'{inputWords[i]}'");
                }
            }
            else if (i < inputWords.Length)
            {
                result.Add($"'{inputWords[i]}'");
            }
            else
            {
                result.Add($"'{originalWords[i]}'");
            }
        }

        var accuracy = (double)matches / originalWords.Length;
        return (string.Join(" ", result), accuracy >= threshold, accuracy);
    }
}