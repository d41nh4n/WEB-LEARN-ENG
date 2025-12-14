namespace Allen.Application;

public interface IGeminiService
{
    IAsyncEnumerable<string> ChatGeminiAsync(string userId, string sessionId, GeminiRequest model);
    IAsyncEnumerable<string> AIAgentSpeakingAsync(string userId, string sessionId, GeminiRequest model);
    Task<SpeakingEvaluationResult?> AIAgentSubmitSpeakingAsync(GeminiRequest model);
    IAsyncEnumerable<string> AIAgentWritingAsync(GeminiRequest model);
    Task<string> AIAgentIeltsWritingAsync(GeminiRequest model);
    Task<string> AIAgentTranslateAsync(GeminiRequest model);
    Task<OperationResult> TranslateAsync(TranslateRequest model);
    //Task<List<GeneratedQuestion>> GenerateQuestionsAsync(QuestionGenerationRequest request);
    Task<string> BuildVocabularyAsync(List<string> listVocabulary);
}