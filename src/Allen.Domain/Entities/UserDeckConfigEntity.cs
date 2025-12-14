//using Allen.Common.Settings.Enum;
//using System.Text.Json;

//namespace Allen.Domain.Entities;

//[Table("UserDeckConfig")]
//public class UserDeckConfigEntity
//{
//    public Guid UserDeckConfigId { get; set; }

//    public DeckType TypeOfDeck { get; set; }

//    // JSON string to store allowed question types
//    public string? AllowTypeOfQuestion { get; set; }

//    public bool IsLearnAllDeck { get; set; } = false;

//    public ShuffleStrategy ShuffleStrategy { get; set; } = ShuffleStrategy.Random;

//    public DateTime? PreferredLearningTime { get; set; }

//    public bool AutoAdvance { get; set; } = false;

//    public int? TimeAutoAdvance { get; set; }

//    public CardDisplayMode CardDisplayMode { get; set; } = CardDisplayMode.FrontOnly;

//    public bool VoiceSupport { get; set; } = false;

//    public int? FontSize { get; set; } = 16;

//    public Theme Theme { get; set; } = Theme.Light;

//    public bool ReminderEnabled { get; set; } = false;

//    public TimeSpan? ReminderTime { get; set; }

//    public LearningGoal LearningGoal { get; set; } = LearningGoal.Balanced;

//    public bool AllowRepeatWrongCards { get; set; } = true;

//    public int? MaxReviewPerSession { get; set; } = 30;

//    // Helper property for JSON serialization
//    [NotMapped]
//    public string[] AllowedQuestionTypes
//    {
//        get => JsonSerializer.Deserialize<string[]>(AllowTypeOfQuestion ?? "[]");
//        set => AllowTypeOfQuestion = JsonSerializer.Serialize(value ?? Array.Empty<string>());
//    }
//}
