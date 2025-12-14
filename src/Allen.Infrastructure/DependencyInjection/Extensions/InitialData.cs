namespace Allen.Infrastructure;

internal class InitialData
{
	public static IEnumerable<RoleEntity> Roles =>
	new List<RoleEntity>
	{
		RoleEntity.Create(new Guid("2bc526a8-452e-499a-b848-bee5531fe1d2"), RoleType.User),
		RoleEntity.Create(new Guid("8b79c8d2-f4d1-4e2d-8f9a-b9f978112d3b"), RoleType.Admin),
		RoleEntity.Create(new Guid("d271e812-08b1-4ea5-b9d9-7c4f536e982c"), RoleType.System),
	};

	public static IEnumerable<UserEntity> Users =>
	new List<UserEntity>
	{
		UserEntity.Create(new Guid("5C235FA5-4AE7-4051-D5EB-08DD5B312E2F"), "DuongHung", "ddhung2003@gmail.com", "!Abc123", false),
		UserEntity.Create(new Guid("AFFF700D-999C-4305-BB76-0E5260BE66C6"), "Admin", "admin@gmail.com", "!Abc123", false),
		UserEntity.Create(new Guid("39E7099D-B699-4F63-A98A-1125F34936B1"), "System", "system@gmail.com", "!Abc123", false),
	};

	public static IEnumerable<UserRoleEntity> UserRoles =>
	new List<UserRoleEntity>
	{
		UserRoleEntity.Create(new Guid("75C5639A-33CD-4F8E-A5D4-5AA900C67AC0"), new Guid("5C235FA5-4AE7-4051-D5EB-08DD5B312E2F"), new Guid("2bc526a8-452e-499a-b848-bee5531fe1d2")),
		UserRoleEntity.Create(new Guid("B73C70A0-B247-43DE-8565-79672465DBB2"), new Guid("AFFF700D-999C-4305-BB76-0E5260BE66C6"), new Guid("8b79c8d2-f4d1-4e2d-8f9a-b9f978112d3b")),
		UserRoleEntity.Create(new Guid("35F905BE-A21E-43E4-B740-81054EE56F3E"), new Guid("39E7099D-B699-4F63-A98A-1125F34936B1"), new Guid("d271e812-08b1-4ea5-b9d9-7c4f536e982c"))
	};
}
