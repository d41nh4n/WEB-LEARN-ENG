namespace Allen.Application;

public class SessionInfo
{
    public string SessionId { get; set; } = string.Empty;
    public string Title { get; set; } = "New conversation";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
