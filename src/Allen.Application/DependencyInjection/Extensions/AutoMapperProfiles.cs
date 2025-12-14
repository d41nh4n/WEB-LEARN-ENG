namespace Allen.Application;

public static class AutoMapperProfiles
{
	public static IMapper Initial()
	{
		var mapperConfiguration = new MapperConfiguration(cfg =>
		{
			cfg.AddProfile<UsersMappingProfile>();
			cfg.AddProfile<LearningUnitsMappingProfile>();
			//cfg.AddProfile<UnitStepMappingProfile>();
            cfg.AddProfile<PostsMappingProfile>();
            cfg.AddProfile<ReadingsMappingProfile>();
            cfg.AddProfile<VocabularyMappingProfile>();
			cfg.AddProfile<TopicsMappingProfile>();
			cfg.AddProfile<CategoriesMappingProfile>();
			cfg.AddProfile<SpeakingsMappingProfile>();
            cfg.AddProfile<ListeningsMappingProfile>();
            cfg.AddProfile<MediasMappingProfile>();
            cfg.AddProfile<QuestionsMappingProfile>();
            cfg.AddProfile<FlashCardsMappingProfile>();
            cfg.AddProfile<FeedbacksMappingProfile>();
            cfg.AddProfile<PushSubscriptionMappingProfile>();
        });

		return mapperConfiguration.CreateMapper();
	}
}
