namespace Allen.Domain;

public class User
{
	public Guid Id { get; set; }
	public string Name { get; set; } = default!;
	public string Email { get; set; } = default!;
	public string Password { get; set; } = default!;
	public string? Picture { get; set; }
	public string? BirthDay { get; private set; }
	public string? Phone { get;  set; }
	public int? Status { get; set; }
	public long? DeleteTime { get; set; }
	public bool IsDeleted { get; set; }
	public string Role { get; set; } = default!;
	public DateTime? CreatedAt { get; set; }
}
