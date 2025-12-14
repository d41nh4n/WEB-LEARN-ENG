namespace Allen.Domain;

public class SubmitSpeakingModel
{
    public Guid TranscriptId { get; set; }
    public required string Text { get; set; }
}
