namespace Allen.Application;

public interface IChatHistoryService
{
    Task AddMessageAsync(string userId, string sessionId, GeminiChatMessage message);
    Task<List<GeminiChatMessage>> GetHistoryAsync(string userId, string sessionId, int takeLast = 20);
    Task ClearHistoryAsync(string userId, string sessionId);

    Task SaveSessionInfoAsync(string userId, SessionInfo info);
    Task<List<SessionInfo>> GetAllSessionsAsync(string userId);
    Task ClearAllSessionsForUserAsync(string userId);
}