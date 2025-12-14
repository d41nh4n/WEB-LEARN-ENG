namespace Allen.Domain;

public class UserInfoModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Picture { get; set; }
    public string? BirthDay { get; set; }
    public string? Phone { get; set; }
    public bool? IsDeleted { get; set; }
    public string Role { get; set; } = string.Empty;
}
