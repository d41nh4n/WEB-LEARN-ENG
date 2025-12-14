//using Allen.Common.Settings.Enum;

//namespace Allen.Domain.Entities;

//[Table("ReviewLog")]
//public class ReviewLogEntity : EntityBase<Guid>
//{
//    // ========== REVIEW METADATA ========== //
//    public DateTime ReviewDate { get; set; } = DateTime.UtcNow;

//    public int Rating { get; set; } // 0: Quên hoàn toàn -> 4: Rất dễ

//    public ReviewContext Context { get; set; } = ReviewContext.Normal;

//    // ========== TIMING METRICS ========== //
//    public double ResponseTimeSec { get; set; } // Thời gian trả lời

//    public double? MediaLoadTimeSec { get; set; } // Thời gian load media
//    public double? ThinkingTimeSec { get; set; } // Thời gian suy nghĩ

//    // ========== FSRS CORE PARAMETERS ========== //
//    public double PreReviewDifficulty { get; set; }
//    public double PreReviewStability { get; set; }
//    public double PostReviewDifficulty { get; set; }
//    public double PostReviewStability { get; set; }
//    public double Retrievability { get; set; }

//    // ========== FSRS PERSONALIZATION ========== //
//    public double UserAbility { get; set; } // Khả năng cá nhân
//    public double ContentComplexity { get; set; } // Độ phức tạp nội dung
//    public double MemoryStrength { get; set; } // Sức mạnh trí nhớ

//    // ========== SPACED REPETITION STATE ========== //
//    public int RepetitionCount { get; set; }
//    public TimeSpan LastInterval { get; set; }
//    public TimeSpan NextInterval { get; set; }
//    public DateTime NextReviewDate { get; set; }

//    // ========== ENVIRONMENT FACTORS ========== //
//    public TimeSpan TimeOfDay { get; set; } // Thời gian trong ngày
//    public DayOfWeek DayOfWeek { get; set; }
//    public int FatigueLevel { get; set; } // 1-10
//    public int ConcentrationLevel { get; set; } // 1-10

//    // ========== CARD SPECIFIC ========== //
//    public string CardType { get; set; } // Loại flashcard
//    public bool IsReverse { get; set; } // Có phải review ngược

//    // ========== NAVIGATION PROPERTIES ========== //
//    public virtual FlashCardEntity Flashcard { get; set; }
//    public virtual UserEntity User { get; set; }
//    public virtual DeckEntity Deck { get; set; }

//    // ========== HELPER METHODS ========== //
//    public bool IsSuccessful => Rating >= 3;
//    public double TotalTimeSec => ResponseTimeSec + (MediaLoadTimeSec ?? 0);
//}
