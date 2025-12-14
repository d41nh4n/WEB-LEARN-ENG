namespace Allen.Domain;

[Table("Users")]
public class UserEntity : EntityBase<Guid>, ISoftDelete
{
    [MaxLength(AppConstants.MaxLengthName)]
    [Required]
    public string Name { get; set; } = string.Empty;
    [MaxLength(AppConstants.MaxLengthName)]
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string Password { get; set; } = string.Empty;
    [MaxLength(AppConstants.MaxLengthUrl)]
    public string? Picture { get; set; }
    public string? BirthDay { get; set; }
    public string? Phone { get; set; }
    public UserStatusType? Status { get; set; }
    public DateTime? DeleteTime { get; set; }
    public bool? IsDeleted { get; set; }
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime RefreshTokenExpiryTime { get; set; }
    public ICollection<UserRoleEntity> UserRoles { get; set; } = [];
    public ICollection<PasswordResetTokenEntity> PasswordResetTokens { get; set; } = [];
    public ICollection<CommentEntity> Comments { get; set; } = [];
    public ICollection<FollowEntity> FollowFollowers { get; set; } = [];
    public ICollection<FollowEntity> Followings { get; set; } = [];

    public ICollection<PostEntity> Posts { get; set; } = [];

    public ICollection<ReactionEntity> Reactions { get; set; } = [];

    //public ICollection<ReadingHighlightEntity> ReadingHighlights { get; set; } = [];

    //public ICollection<SpeakingPracticeEntity> SpeakingPractices { get; set; } = [];

    public ICollection<UserBlockEntity> UserBlockBlockedByUsers { get; set; } = [];
    public ICollection<UserBlockEntity> UserBlockBlockedUsers { get; set; } = [];
    public ICollection<UserAnswerEntity> UserAnswers { get; set; } = [];
    //public ICollection<UserUnitProgressEntity> UserUnitProgresses { get; set; } = [];

    public ICollection<UserVocabularyEntity> UserVocabularies { get; set; } = []        ;

    public ICollection<VocabularyProgressEntity> VocabularyProgresses { get; set; } = [];

    public ICollection<WritingSubmissionEntity> WritingSubmissions { get; set; } = [];

    public ICollection<PaymentEntity> Payments { get; set; } = [];

    public ICollection<UserPointTransactionEntity> UserPointTransactions { get; set; } = [];
    public UserPointEntity? UserPoint { get; set; }

    public ICollection<PushSubscriptionEntity>? PushSubscriptions { get; set; }

    public static UserEntity Create(Guid id, string name, string email, string password, bool isDeleted)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        var userEntity = new UserEntity
        {
            Id = id,
            Name = name,
            Password = BCrypt.Net.BCrypt.HashPassword(password),
            Email = email,
            IsDeleted = isDeleted,
		};

        return userEntity;
    }
}
