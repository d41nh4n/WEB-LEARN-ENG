namespace Allen.Domain;

public class ReviewHistoryModel
{
    public Guid Id { get; set; }
    public Guid? FlashCardStateId { get; set; }
    public DateTime ReviewDate { get; set; }
    public RatingLearningCard Rating { get; set; }
    public double StabilityAtReview { get; set; }
    public double DifficultyAtReview { get; set; }
}
