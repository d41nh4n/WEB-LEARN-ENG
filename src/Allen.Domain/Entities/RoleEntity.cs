namespace Allen.Domain;

[Table("Roles")]
public class RoleEntity : EntityBase<Guid>
{
	[MaxLength(AppConstants.MaxLengthName)]
	public RoleType Name { get; set; }
	public virtual ICollection<UserRoleEntity> UserRoles { get; set; } = [];
	public virtual ICollection<RolePermissionEntity> RolePermissions { get; set; } = [];
	public static RoleEntity Create(Guid id, RoleType name)
	{
		var roleEntity = new RoleEntity
		{
			Id = id,
			Name = name
		};
		return roleEntity;
	}
}
