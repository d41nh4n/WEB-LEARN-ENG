namespace Allen.Domain;

public class FlashCardStateModel
{
    public Guid Id { get; set; }
    public Guid FlashCardId { get; set; }

    public double? Stability { get; set; }
    public double? Difficulty { get; set; }
    public int Repetition { get; set; } = 0;
    public int Interval { get; set; } = 0;
    public DateTime NextReviewDate { get; set; } = DateTime.UtcNow;
    public DateTime? LastReviewedAt { get; set; }
    public RatingLearningCard? LastRating { get; set; }

    public static FlashCardStateModel CreateNew(Guid flashCardId)
    {
        return new FlashCardStateModel
        {
            Id = Guid.NewGuid(),
            FlashCardId = flashCardId,
            Stability = 0,
            Difficulty = 0,
            Repetition = 0,
            Interval = 0,
            NextReviewDate = DateTime.UtcNow,
            LastReviewedAt = null,
            LastRating = null
        };
    }

}
