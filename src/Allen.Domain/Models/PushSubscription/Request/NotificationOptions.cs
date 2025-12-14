namespace Allen.Domain;

public class NotificationData
{
    public string Url { get; set; } = "/";
}

public class NotificationOptions
{
    public string Title { get; set; } = "Test Notification";

    public string Body { get; set; } = "This is a test message from the server.";

    public string Icon { get; set; } = "/icon.png";

    public string Tag { get; set; } = "generic-notification-tag";

    public bool Renotify { get; set; } = true;

    public NotificationData Data { get; set; } = new NotificationData();
}

public class PushNotificationPayload
{
    public NotificationOptions Notification { get; set; } = new NotificationOptions();

    public int TTLInSeconds { get; set; } = 86400;

    public string Priority { get; set; } = "normal";
}