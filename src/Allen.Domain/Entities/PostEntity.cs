namespace Allen.Domain;

[Table("Posts")]
public class PostEntity : EntityBase<Guid>
{
	public Guid UserId { get; set; }
	public UserEntity User { get; set; } = null!; 
	public string Content { get; set; } = null!;
	public string? Medias { get; set; }
	public PrivacyType Privacy { get; set; }
}