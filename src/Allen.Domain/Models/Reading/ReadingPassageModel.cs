namespace Allen.Domain;

public class ReadingPassageModel
{
    public Guid Id { get; set; }
    public Guid LearningUnitId { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    //public string? Difficulty { get; set; }
    public DateTime? CreatedAt { get; set; }
    public List<QuestionGroupModel> GroupedQuestions { get; set; } = [];
}
public class QuestionGroupModel
{
    public string QuestionType { get; set; } = null!;
    public List<QuestionModel> Questions { get; set; } = [];
}
