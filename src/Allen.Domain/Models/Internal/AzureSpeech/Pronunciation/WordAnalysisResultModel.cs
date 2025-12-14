namespace Allen.Domain;

public class WordAnalysisResult
{
    public string Word { get; set; } = string.Empty;
    public double AccuracyScore { get; set; }
    public string ErrorType { get; set; } = string.Empty;
    public List<PhonemeAnalysisModel> PhonemeAnalysis { get; set; } = new();
}
