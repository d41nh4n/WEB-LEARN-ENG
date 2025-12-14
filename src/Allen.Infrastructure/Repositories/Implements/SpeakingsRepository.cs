namespace Allen.Infrastructure;

[RegisterService(typeof(ISpeakingsRepository))]
public class SpeakingsRepository(SqlApplicationDbContext context) : RepositoryBase<SpeakingEntity>(context), ISpeakingsRepository
{
    private readonly SqlApplicationDbContext _context = context;

    public async Task<SpeakingModel> GetByLearningUnitIdAsync(Guid learningUnitId)
    {
        return await (from speaking in _context.Speakings.AsNoTracking()
                    join media in _context.Medias.AsNoTracking() on speaking.MediaId equals media.Id
                    join TranscriptEntity transcript in _context.Transcripts.AsNoTracking() on media.Id equals transcript.MediaId into transcriptGroup
                    where speaking.LearningUnitId == learningUnitId
                    select new SpeakingModel
                    {
                        Id = speaking.Id,
                        LearningUnitId = speaking.LearningUnitId,
                        Media = new MediaModel
                        {
                            Id = media.Id,
                            Title = media.Title,
                            MediaType = media.MediaType.ToString(),
                            SourceUrl = media.SourceUrl,
                            Transcripts = new QueryResult<TranscriptModel>
                            {
                                Data = transcriptGroup.Select(t => new TranscriptModel
                                {
                                    Id = t.Id,
                                    ContentEN = t.ContentEN,
                                    ContentVN = t.ContentVN,
                                    StartTime = t.StartTime,
                                    EndTime = t.EndTime,
                                    IPA = t.IPA,
                                    OrderIndex = t.OrderIndex
                                }).ToList(),
                                TotalCount = transcriptGroup.Count()
                            }
                        }
                    }).FirstOrDefaultAsync() ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(SpeakingEntity), learningUnitId));
    }

	public async Task<List<SpeakingForIeltsModel>> GetByLearningUnitIdForIeltsAsync(
	Guid learningUnitId,
	GetByLearningUnitIdForIeltsQuery query)
	{
		var speakingsQuery = _context.Speakings
			.AsNoTracking()
			.GroupJoin(
				_context.Questions.AsNoTracking(),
				speaking => speaking.Id,               // khóa join
				question => question.ModuleItemId,     // khóa đối chiếu
				(speaking, questions) => new { speaking, questions }
			)
			.SelectMany(
				sq => sq.questions.DefaultIfEmpty(),   // LEFT JOIN
				(sq, question) => new { sq.speaking, question }
			)
			.Where(s => s.speaking.LearningUnitId == learningUnitId);

		if (query.SectionIndex != null && query.SectionIndex.Any())
		{
			speakingsQuery = speakingsQuery
				.Where(s => s.speaking.SectionIndex.HasValue &&
							query.SectionIndex.Contains(s.speaking.SectionIndex.Value));
		}

		var result = await speakingsQuery
			.Select(s => new SpeakingForIeltsModel
			{
				Id = s.speaking.Id,
				LearningUnitId = s.speaking.LearningUnitId,
				SectionIndex = s.speaking.SectionIndex,
				Question = s.question == null ? null : new QuestionForSpeakingModel
				{
					Id = s.question.Id,
					ModuleItemId = s.question.ModuleItemId,
					Label = s.question.Label ?? string.Empty, 
					Prompt = s.question.Prompt ?? string.Empty 
				}
			})
			.OrderBy(s => s.SectionIndex)
			.ToListAsync();

		return result;
	}



	public async Task<TranscriptForSubmitModel> GetTranscriptByIdAsync(Guid transcriptId)
    {
        return await _context.Transcripts.AsNoTracking()
             .Where(x => x.Id == transcriptId)
             .Select(x => new TranscriptForSubmitModel
             {
                 ContentEN = x.ContentEN
             })
             .FirstOrDefaultAsync() ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(TranscriptEntity), transcriptId));
    }
}
