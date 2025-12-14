namespace Allen.Domain;

public class GeminiResponse
{
    public List<Candidate> Candidates { get; set; } = new();
}

public class Candidate
{
    public Content Content { get; set; } = new();
}

public class Content
{
    public List<Part> Parts { get; set; } = new();
}

public class Part
{
    public string Text { get; set; } = string.Empty;
}

public class GeminiFeedback
{
    public string Improvements { get; set; } = string.Empty;
    public string Comments { get; set; } = string.Empty;
}

public class GeminiOutOfScope
{
    public string Message { get; set; } = string.Empty;
}