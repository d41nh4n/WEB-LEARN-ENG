namespace Allen.Infrastructure;

public class SqlApplicationDbContext(DbContextOptions<SqlApplicationDbContext> options) : DbContext(options)
{
    public DbSet<UserEntity> Users { get; set; }
    public DbSet<UserRoleEntity> UserRoles { get; set; }
    public DbSet<RoleEntity> Roles { get; set; }
    public DbSet<RoleHierarchyEntity> RoleHierarchies { get; set; }
    public DbSet<PermissionEntity> Permissions { get; set; }
    public DbSet<RolePermissionEntity> RolePermissions { get; set; }
    public DbSet<PasswordResetTokenEntity> PasswordResetTokens { get; set; }
    public DbSet<CommentEntity> Comments { get; set; }
    public DbSet<FollowEntity> Follows { get; set; }
    public DbSet<LearningUnitEntity> LearningUnits { get; set; }
    public DbSet<ListeningEntity> Listenings { get; set; }
    public DbSet<PostEntity> Posts { get; set; }
    public DbSet<QuestionEntity> Questions { get; set; }
    public DbSet<SubQuestionEntity> SubQuestions { get; set; }
    public DbSet<ReactionEntity> Reactions { get; set; }
    //public DbSet<ReadingHighlightEntity> ReadingHighlights { get; set; }
    public DbSet<ReadingPassageEntity> ReadingPassages { get; set; }
    //public DbSet<SpeakingPracticeEntity> SpeakingPractices { get; set; }
    //public DbSet<UnitContentEntity> UnitContents { get; set; }
    //public DbSet<UnitStepEntity> UnitSteps { get; set; }
    public DbSet<UserBlockEntity> UserBlocks { get; set; }
    public DbSet<UserAnswerEntity> UserAnswers { get; set; }
    //public DbSet<UserUnitProgressEntity> UserUnitProgresses { get; set; }
    //public DbSet<UserStepProgressEntity> UserStepProgresses { get; set; }
    public DbSet<UserVocabularyEntity> UserVocabularies { get; set; }
    public DbSet<VocabularyEntity> Vocabularies { get; set; }
    public DbSet<VocabularyProgressEntity> VocabularyProgresses { get; set; }
    public DbSet<WritingSubmissionEntity> WritingSubmissions { get; set; }
    public DbSet<VocabularyRelationEntity> VocabularyRelations { get; set; }
    public DbSet<VocabularyMeaningEntity> VocabularyMeanings { get; set; }
    public DbSet<TopicEntity> Topics { get; set; }
    public DbSet<TagEntity> Tags { get; set; }
    public DbSet<VocabularyTagEntity> VocabularyTags { get; set; }
    public DbSet<CategoryEntity> Categories { get; set; }
    public DbSet<WritingEntity> Writings { get; set; }
    public DbSet<TranscriptEntity> Transcripts { get; set; }
    public DbSet<MediaEntity> Medias { get; set; }
    public DbSet<SpeakingEntity> Speakings { get; set; }
    public DbSet<PaymentEntity> Payments { get; set; }
    public DbSet<PackageEntity> Packages { get; set; }
    public DbSet<UserPointEntity> UserPoints { get; set; }
    public DbSet<UserPointTransactionEntity> UserPointTransactions { get; set; }
    public DbSet<UserTestAttemptEntity> UserTestAttempts { get; set; }
    public DbSet<NotificationEntity> Notifications { get; set; }
    public DbSet<DeckEntity> Decks { get; set; }
    public DbSet<FlashCardEntity> FlashCards { get; set; }
    public DbSet<FlashCardStateEntity> FlashCardStates { get; set; }
    public DbSet<ReviewFLHistoryEntity> ReviewFLHistory { get; set; }
    public DbSet<FeedbackEntity> Feedbacks { get; set; }
    public DbSet<PushSubscriptionEntity> PushSubscriptions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppSqlDbContext).Assembly);

        modelBuilder.Entity<PasswordResetTokenEntity>()
            .HasOne(p => p.User)
            .WithMany(u => u.PasswordResetTokens)
            .HasForeignKey(p => p.UserId);

        modelBuilder.Entity<FollowEntity>().HasKey(f => new { f.Id, f.FollowerId, f.FollowingId });

        modelBuilder.Entity<FollowEntity>().HasOne(f => f.Follower)
             .WithMany(u => u.Followings)
             .HasForeignKey(f => f.FollowerId)
             .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<FollowEntity>().HasOne(f => f.Following)
             .WithMany(u => u.FollowFollowers)
             .HasForeignKey(f => f.FollowingId)
             .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<UserBlockEntity>().HasOne(x => x.BlockedByUser)
            .WithMany()
            .HasForeignKey(x => x.BlockedByUserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<UserBlockEntity>().HasOne(x => x.BlockedUser)
            .WithMany()
            .HasForeignKey(x => x.BlockedUserId)
            .OnDelete(DeleteBehavior.NoAction);


        modelBuilder.Entity<UserBlockEntity>().HasIndex(x => new { x.BlockedByUserId, x.BlockedUserId }).IsUnique();

        modelBuilder.Entity<RoleHierarchyEntity>().HasOne(x => x.ParentRole)
            .WithMany()
            .HasForeignKey(x => x.ParentRoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RoleHierarchyEntity>().HasOne(x => x.ChildRole)
            .WithMany()
            .HasForeignKey(x => x.ChildRoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserBlockEntity>().HasOne(x => x.BlockedByUser)
            .WithMany(u => u.UserBlockBlockedByUsers)
            .HasForeignKey(x => x.BlockedByUserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<UserBlockEntity>().HasOne(x => x.BlockedUser)
            .WithMany(u => u.UserBlockBlockedUsers)
            .HasForeignKey(x => x.BlockedUserId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<UserBlockEntity>().HasIndex(x => new { x.BlockedByUserId, x.BlockedUserId }).IsUnique();

        modelBuilder.Entity<VocabularyRelationEntity>()
            .HasOne(x => x.SourceVocab)
            .WithMany()
            .HasForeignKey(x => x.SourceVocabId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<VocabularyRelationEntity>()
            .HasOne(x => x.RelateVocab)
            .WithMany()
            .HasForeignKey(x => x.RelateVocabId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<VocabularyTagEntity>()
           .HasOne(vt => vt.Vocabulary)
           .WithMany(v => v.VocabularyTags)
           .HasForeignKey(vt => vt.VocabularyId)
           .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<VocabularyTagEntity>()
            .HasOne(vt => vt.Tag)
            .WithMany(t => t.VocabularyTags)
            .HasForeignKey(vt => vt.TagId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<VocabularyEntity>()
            .HasOne(v => v.Topic)
            .WithMany(t => t.Vocabularies)
            .HasForeignKey(v => v.TopicId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<WritingSubmissionEntity>()
            .HasOne(s => s.Attempt)
            .WithMany(a => a.WritingSubmissions)
            .HasForeignKey(s => s.AttemptId)
            .OnDelete(DeleteBehavior.Restrict); // hoặc NoAction


        //modelBuilder.Entity<SpeakingPracticeEntity>()
        //    .HasOne(sp => sp.SpeakingEntity)
        //    .WithMany(s => s.SpeakingPractices)
        //    .HasForeignKey(sp => sp.SpeakingId);

        modelBuilder.Entity<DeckEntity>()
           .HasOne(d => d.UserCreate)
           .WithMany()
           .HasForeignKey(d => d.UserCreateId)
           .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<DeckEntity>()
        .HasIndex(d => d.UserCreateId);

        modelBuilder.Entity<FlashCardEntity>()
            .HasOne(fc => fc.Deck)
            .WithMany(d => d.FlashCards)
            .HasForeignKey(fc => fc.DeckId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FlashCardEntity>()
        .HasIndex(fc => new { fc.DeckId, fc.CreatedAt });

        modelBuilder.Entity<FlashCardEntity>()
       .HasIndex(fc => fc.RelationVocabularyCardId);

        modelBuilder.Entity<FlashCardStateEntity>()
            .HasOne(cs => cs.FlashCard)
            .WithOne(fc => fc.CardState)
            .HasForeignKey<FlashCardStateEntity>(cs => cs.FlashCardId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<FlashCardStateEntity>()
        .HasIndex(cs => new { cs.Repetition, cs.NextReviewDate });

        modelBuilder.Entity<FlashCardStateEntity>()
            .HasIndex(cs => cs.FlashCardId)
            .IsUnique();

        modelBuilder.Entity<ReviewFLHistoryEntity>()
            .HasOne(rh => rh.FlashCardState)
            .WithMany(cs => cs.ReviewHistories)
            .HasForeignKey(rh => rh.FlashCardStateId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ReviewFLHistoryEntity>()
        .HasIndex(rh => rh.FlashCardStateId);

        modelBuilder.Entity<PushSubscriptionEntity>()
            .HasOne(d => d.User)
            .WithMany(u => u.PushSubscriptions)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}
