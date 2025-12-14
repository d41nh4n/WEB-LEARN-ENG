namespace Allen.Domain;

public class CreateListeningsForIeltsModel
{
    public CreateLearningUnitForWritingModel LearningUnit { get; set; } = null!;
    public List<CreateListeningSectionModel> Data { get; set; } = null!;
}
public class CreateListeningSectionModel
{
    public int SectionIndex { get; set; }
    public int EstimatedReadingTime { get; set; }
    public CreateMediaWithoutTranscriptModel Media { get; set; } = null!;
    public List<CreateQuestionForListeningIeltsModel> Questions { get; set; } = [];
}