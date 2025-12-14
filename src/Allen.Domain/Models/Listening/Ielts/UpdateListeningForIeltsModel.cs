namespace Allen.Domain;

public class UpdateListeningForIeltsModel
{
    public UpdateLearningUnitForWritingModel LearningUnit { get; set; } = null!;
    public List<UpdateListeningSectionModel> Data { get; set; } = null!;
}
public class UpdateListeningSectionModel
{
    public int SectionIndex { get; set; }
    public int EstimatedReadingTime { get; set; }
    public UpdateMediaWithoutTranscriptModel Media { get; set; } = null!;
    public List<UpdateQuestionModel> Questions { get; set; } = [];
}
