namespace Allen.Infrastructure;

[RegisterService(typeof(IVocabularyRepository))]
public class VocabularyRepository(
	SqlApplicationDbContext context
) : RepositoryBase<VocabularyEntity>(context), IVocabularyRepository
{
	private readonly SqlApplicationDbContext _context = context;

    public async Task<QueryResult<VocabulariesModel>> GetVocabulariesAsync(QueryInfo queryInfo)
    {
        string searchText = queryInfo.SearchText?.ToLower() ?? "";
        var query = (from vocabulary in _context
                                 .Vocabularies.AsNoTracking()
                                 .Where(v => v.Word.ToLower().Contains(searchText))
                     select new VocabulariesModel
                     {
                         Id = vocabulary.Id,
                         Level = vocabulary.Level.ToString(),
                         Word = vocabulary.Word
                     });

        var entities = await query
            .OrderBy(x => x.Word)
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();
		return new QueryResult<VocabulariesModel>
		{
			Data = entities,
			TotalCount = queryInfo.NeedTotalCount
				? await query.CountAsync()
				: 0
		};
	}

	public async Task<VocabularyModel> GetVocabularyByIdAsync(Guid vocabId)
	{
		var vocabulary = await _context.Vocabularies
		.AsNoTracking()
		.Include(v => v.Topic)
		.Include(v => v.VocabularyMeanings)
		.FirstOrDefaultAsync(v => v.Id == vocabId);

		if (vocabulary == null)
			throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(VocabularyEntity), vocabId));

		return new VocabularyModel
		{
			Id = vocabulary.Id,
			Topic = vocabulary.Topic != null ? new VocabularyTopicModel
			{
				Id = vocabulary.Topic.Id,
				TopicName = vocabulary.Topic.TopicName
			} : null,
			Word = vocabulary.Word,
			Level = vocabulary.Level.ToString(),
			VocabularyMeanings = vocabulary.VocabularyMeanings?.Select(meaning => new VocabularyMeaningModel
			{
				Id = meaning.Id,
				PartOfSpeech = meaning.PartOfSpeech.ToString(),
				Pronunciation = meaning.Pronunciation,
				Audio = meaning.Audio,
				DefinitionEN = meaning.DefinitionEN,
				DefinitionVN = meaning.DefinitionVN,
				Example1 = meaning.Example1,
				Example2 = meaning.Example2,
				Example3 = meaning.Example3
			}).ToList() ?? new List<VocabularyMeaningModel>()
		};
	}

	public async Task<VocabularyModel> GetVocabularyByWordAsync(string word)
	{
		var vocabulary = await _context.Vocabularies
		 .AsNoTracking()
		 .Include(v => v.Topic)
		 .Include(v => v.VocabularyMeanings)
		 .FirstOrDefaultAsync(v => v.Word == word);

		if (vocabulary == null)
			throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(VocabularyEntity), word));

		return new VocabularyModel
		{
			Id = vocabulary.Id,
			Topic = vocabulary.Topic != null ? new VocabularyTopicModel
			{
				Id = vocabulary.Topic.Id,
				TopicName = vocabulary.Topic.TopicName
			} : null,
			Word = vocabulary.Word,
			Level = vocabulary.Level.ToString(),
			VocabularyMeanings = vocabulary.VocabularyMeanings?.Select(meaning => new VocabularyMeaningModel
			{
				Id = meaning.Id,
				PartOfSpeech = meaning.PartOfSpeech.ToString(),
				Pronunciation = meaning.Pronunciation,
				Audio = meaning.Audio,
				DefinitionEN = meaning.DefinitionEN,
				DefinitionVN = meaning.DefinitionVN,
				Example1 = meaning.Example1,
				Example2 = meaning.Example2,
				Example3 = meaning.Example3
			}).ToList() ?? new List<VocabularyMeaningModel>()
		};
	}

	public async Task<QuizVocabulariesResponeModel> GetQuizVocabulariesAsync(QuizVocabulariesRequestModel model)
	{
		var allSelectedIds = new List<Guid>();

		var levelWordCounts = new Dictionary<LevelType, int>
		{
			{ LevelType.A1, model.NumberA1Words },
			{ LevelType.A2, model.NumberA2Words },
			{ LevelType.B1, model.NumberB1Words },
			{ LevelType.B2, model.NumberB2Words },
			{ LevelType.C1, model.NumberC1Words },
			{ LevelType.C2, model.NumberC2Words }
		};

		foreach (var (level, count) in levelWordCounts)
		{
			if (count > 0)
			{
				var randomIds = await GetRandomVocabularyIdsByLevelAsync(model.Topic, level, count);
				allSelectedIds.AddRange(randomIds);
			}
		}

		if (allSelectedIds.Count == 0)
		{
			return new QuizVocabulariesResponeModel { Vocabularies = new List<VocabularyModel>() };
		}

		var allVocabularies = await _context.Vocabularies
			.AsNoTracking()
			.Where(v => allSelectedIds.Contains(v.Id))
			.Include(v => v.Topic)
			.Include(v => v.VocabularyMeanings)
			.ToListAsync();

		var vocabularyModels = allVocabularies
			.Select(vocabulary => new VocabularyModel
			{
				Id = vocabulary.Id,
				Topic = vocabulary.Topic != null ? new VocabularyTopicModel
				{
					Id = vocabulary.Topic.Id,
					TopicName = vocabulary.Topic.TopicName
				} : null,
				Word = vocabulary.Word,
				Level = vocabulary.Level.ToString(),
				VocabularyMeanings = vocabulary.VocabularyMeanings?.Select(meaning => new VocabularyMeaningModel
				{
					Id = meaning.Id,
					PartOfSpeech = meaning.PartOfSpeech.ToString(),
					Pronunciation = meaning.Pronunciation,
					Audio = meaning.Audio,
					DefinitionEN = meaning.DefinitionEN,
					DefinitionVN = meaning.DefinitionVN,
					Example1 = meaning.Example1,
					Example2 = meaning.Example2,
					Example3 = meaning.Example3
				}).ToList() ?? []
			}).ToList(); ;

		return new QuizVocabulariesResponeModel
		{
			Vocabularies = vocabularyModels
		};
	}

	private async Task<List<Guid>> GetRandomVocabularyIdsByLevelAsync(Guid? topicId, LevelType level, int count)
	{
		var query = _context.Vocabularies
			.AsNoTracking()
			.Where(v => v.Level == level);

		if (topicId.HasValue)
		{
			query = query.Where(v => v.TopicId == topicId.Value);
		}

		var randomIds = await query
			.OrderBy(r => Guid.NewGuid())
			.Take(count)
			.Select(v => v.Id)
			.ToListAsync();

		return randomIds;
	}

	public async Task<List<VocabularyEntity>> GetVocabulariesByIdsAsync(List<Guid> vocabIds)
	{
		return await _context.Vocabularies
			.AsNoTracking()
			.Include(v => v.VocabularyMeanings)
			.Where(v => vocabIds.Contains(v.Id))
			.ToListAsync() ?? throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(VocabularyEntity)));
	}

	public async Task<QueryResult<VocabularyModel>> GetVocabulariesByTopicIdAsync(QueryInfo queryInfo, Guid topic)
	{
		var vocabularyQuery = _context.Vocabularies
			.AsNoTracking()
			.Where(v => v.TopicId == topic);

		if (!string.IsNullOrWhiteSpace(queryInfo.SearchText))
		{
			var searchText = queryInfo.SearchText.ToLower();
			vocabularyQuery = vocabularyQuery.Where(v => v.Word.ToLower().Contains(searchText));
		}
		var query = vocabularyQuery
			.Select(vocabulary => new VocabularyModel
			{
				Id = vocabulary.Id,
				Topic = vocabulary.Topic != null ? new VocabularyTopicModel
				{
					Id = vocabulary.Topic.Id,
					TopicName = vocabulary.Topic.TopicName
				} : null,
				Word = vocabulary.Word,
				Level = vocabulary.Level.ToString(),

				VocabularyMeanings = vocabulary.VocabularyMeanings
					.Select(meaning => new VocabularyMeaningModel
					{
						Id = meaning.Id,
						PartOfSpeech = meaning.PartOfSpeech.ToString(),
						Pronunciation = meaning.Pronunciation,
						Audio = meaning.Audio,
						DefinitionEN = meaning.DefinitionEN,
						DefinitionVN = meaning.DefinitionVN,
						Example1 = meaning.Example1,
						Example2 = meaning.Example2,
						Example3 = meaning.Example3
					}).ToList() ?? new List<VocabularyMeaningModel>()
			});

        var entities = await query
            .OrderByDescending(x => x.Word) 
            .Skip(queryInfo.Skip)
            .Take(queryInfo.Top)
            .ToListAsync();

        return new QueryResult<VocabularyModel>
        {
            Data = entities,
            TotalCount = queryInfo.NeedTotalCount
                ? await vocabularyQuery.CountAsync() 
                : 0
        };
    }

    public async Task<List<VocabularyEntity>> GetVocabulariesByWordsAsync(List<string> words)
    {
        return await _context.Vocabularies
            .AsNoTracking()
            .Where(v => words.Contains(v.Word))
            .ToListAsync();
    }
}

