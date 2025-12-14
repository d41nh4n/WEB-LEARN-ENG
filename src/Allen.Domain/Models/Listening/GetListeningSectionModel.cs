namespace Allen.Domain;

public class GetListeningSectionModel
{
    public Guid ListeningId { get; set; }
	public int SectionIndex { get; set; }
    public GetMediaWithoutTranscriptModel Media { get; set; } = null!;
    public List<QuestionModel> Questions { get; set; } = [];
}
