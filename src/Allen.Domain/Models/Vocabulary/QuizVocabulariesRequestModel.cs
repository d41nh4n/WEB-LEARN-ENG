namespace Allen.Domain;
public class QuizVocabulariesRequestModel
{
    public Guid? Topic { get; set; }

    public int NumberA1Words { get; set; } = 0;

    public int NumberA2Words { get; set; } = 0;

    public int NumberB1Words { get; set; } = 0;

    public int NumberB2Words { get; set; } = 0;

    public int NumberC1Words { get; set; } = 0;

    public int NumberC2Words { get; set; } = 0;
}

