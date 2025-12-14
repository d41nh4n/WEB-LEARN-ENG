namespace Allen.Domain;

public class ListeningModel
{
    public Guid Id { get; set; }
    public Guid LearningUnitId { get; set; }
    public MediaModel? Media { get; set; }
}
