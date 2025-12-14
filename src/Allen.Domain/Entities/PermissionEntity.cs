namespace Allen.Domain;

[Table("Permissions")]
public class PermissionEntity : EntityBase<Guid>
{
	[MaxLength(AppConstants.MaxLengthPermision)]
	public string Resource { get; set; } = string.Empty;
	[MaxLength(AppConstants.MaxLengthPermision)]
	public string Action { get; set; } = string.Empty;
	public virtual ICollection<RolePermissionEntity> RolePermissions { get; set; } = [];
}
