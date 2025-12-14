namespace Allen.Domain;

public class CreateReadingPassageModel
{
    [JsonIgnore]
    public Guid LearningUnitId { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public int? EstimatedReadingTime { get; set; }
    public CreateLearningUnitForReadingModel? LearningUnit { get; set; }
    public List<CreateOrUpdateQuestionForReadingModel> Questions { get; set; } = [];
}
