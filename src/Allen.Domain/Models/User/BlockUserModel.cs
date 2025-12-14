namespace Allen.Domain;

public class BlockUserModel
{
    [JsonIgnore]
    public Guid BlockedByUserId { get; set; }
    public Guid BlockedUserId { get; set; }
}
