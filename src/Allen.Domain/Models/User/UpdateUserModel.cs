namespace Allen.Domain;

public class UpdateUserModel
{
	public string? Name { get; set; }
	public IFormFile? Picture { get; set; }
    public string? BirthDay { get; set; }
    public string? Phone { get; set; }
}