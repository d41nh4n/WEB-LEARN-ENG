namespace Allen.Domain;

public class FlashCardStudyQueueLstModel
{
    public List<FlashCardStudyModel> NewFlashCard { get; set; } = [];

    public List<FlashCardStudyModel> ReviewFlashCard { get; set; } = [];
}
