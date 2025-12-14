namespace Allen.Application;

[RegisterService(typeof(IChatHistoryService))]
public class ChatHistoryService(
    IRedisService _redis
) : IChatHistoryService
{
    private readonly TimeSpan _expiry = TimeSpan.FromHours(1);

    private string BuildKey(string userId, string sessionId)
        => $"chat:{userId}:{sessionId}";

    private string BuildSessionKey(string userId)
        => $"sessions:{userId}";

    private string BuildSessionInfoKey(string userId, string sessionId)
        => $"sessions:{userId}:{sessionId}";

    // Thêm tin nhắn, set expiry
    public async Task AddMessageAsync(string userId, string sessionId, GeminiChatMessage message)
    {
        var key = BuildKey(userId, sessionId);
        var json = JsonHelper.Serialize(message);
        await _redis.PushRightAsync(key, json ?? "");
        await _redis.SetExpiryAsync(key, _expiry);
    }

    // Lấy lịch sử chat
    public async Task<List<GeminiChatMessage>> GetHistoryAsync(string userId, string sessionId, int takeLast = 20)
    {
        var key = BuildKey(userId, sessionId);
        var values = await _redis.ListRangeAsync(key, -takeLast, -1);
        return values
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .Select(v => JsonHelper.Deserialize<GeminiChatMessage>(v))
            .OfType<GeminiChatMessage>().ToList();
    }

    // Lấy tất cả session, lọc loại session đã hết hạn (sessionInfo == null)
    public async Task<List<SessionInfo>> GetAllSessionsAsync(string userId)
    {
        var listKey = BuildSessionKey(userId);
        var sessionIds = await _redis.GetSetMembersAsync(listKey);

        var result = new List<SessionInfo>();
        foreach (var id in sessionIds)
        {
            var infoKey = BuildSessionInfoKey(userId, id.ToString());
            var info = await _redis.GetAsync<SessionInfo>(infoKey);
            if (info != null)
                result.Add(info);
            else
                // Xóa sessionId rác khỏi set
                await _redis.RemoveFromSetAsync(listKey, id);
        }

        return result.OrderByDescending(x => x.CreatedAt).ToList();
    }

    // Lưu session info với expiry
    public async Task SaveSessionInfoAsync(string userId, SessionInfo info)
    {
        var infoKey = BuildSessionInfoKey(userId, info.SessionId);
        await _redis.SetAsync(infoKey, info, _expiry);

        var listKey = BuildSessionKey(userId);
        await _redis.AddToSetAsync(listKey, info.SessionId);
    }

    // Xóa toàn bộ session user sạch trong Redis
    public async Task ClearAllSessionsForUserAsync(string userId)
    {
        var listKey = BuildSessionKey(userId);
        var sessionIds = await _redis.GetSetMembersAsync(listKey);

        foreach (var sessionId in sessionIds)
        {
            var chatKey = BuildKey(userId, sessionId);
            var infoKey = BuildSessionInfoKey(userId, sessionId);

            await _redis.DeleteAsync(chatKey);
            await _redis.DeleteAsync(infoKey);
        }

        await _redis.DeleteAsync(listKey);
    }

    // Xóa lịch sử chat của 1 session
    public async Task ClearHistoryAsync(string userId, string sessionId)
    {
        var key = BuildKey(userId, sessionId);
        await _redis.DeleteAsync(key);
    }
}
