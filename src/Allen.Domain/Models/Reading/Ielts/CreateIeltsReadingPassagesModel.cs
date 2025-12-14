namespace Allen.Domain;

public class CreateIeltsReadingPassagesModel
{
    [JsonIgnore]
    public Guid LearningUnitId { get; set; }
    public string Content { get; set; } = null!;
    public int? EstimatedReadingTime { get; set; }
    public required CreateLearningUnitForReadingModel LearningUnit { get; set; } = null!;
    // Danh sách câu hỏi cho passage
    public List<CreateQuestionModel> Questions { get; set; } = new();
}
