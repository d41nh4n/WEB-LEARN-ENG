namespace Allen.Domain;

public class PronunciationAnalysisResultModel
{
    public string TranscribedText { get; set; } = string.Empty;
    public string ReferenceText { get; set; } = string.Empty;
    //public double OverallScore { get; set; }
    public double OverallBand { get; set; }
    public double AccuracyScore { get; set; }
    public double FluencyScore { get; set; }
    public double CompletenessScore { get; set; }
    public double PronDetailScore { get; set; }
    public string? PronunciationFeedback { get; set; }
	public List<WordAnalysisResult> WordAnalysis { get; set; } = new();
}
