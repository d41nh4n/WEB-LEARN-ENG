namespace Allen.Domain;

public class SpeakingEvaluationResult
{
	public CriterionScore TaskResponse { get; set; } = new();
	public CriterionScore FluencyCoherence { get; set; } = new();
	public CriterionScore LexicalResource { get; set; } = new();
	public CriterionScore GrammarAccuracy { get; set; } = new();
}

public class CriterionScore
{
	public double Score { get; set; }
	public string Feedback { get; set; } = string.Empty;
}
