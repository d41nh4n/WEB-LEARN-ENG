namespace Allen.Domain;

public class ListeningForIeltsModel
{
    public Guid LearningUnitId { get; set; }
    public string Title { get; set; } = null!;
    public int EstimatedReadingTime { get; set; }
    public List<GetListeningSectionModel> Data { get; set; } = null!;
}
