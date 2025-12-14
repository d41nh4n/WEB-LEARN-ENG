namespace Allen.Application;

[RegisterService(typeof(IRolesService))]
public class RolesService(
	IRolesRepository _repository,
	IUsersRepository _usersRepository,
	IUnitOfWork _unitOfWork,
	IMapper _mapper) : IRolesService
{
	public async Task<List<Role>> GetRolesAsync()
	{
		var roles = await _unitOfWork.Repository<RoleEntity>().GetAllAsync();
		return _mapper.Map<List<Role>>(roles);
	}
	public async Task<UserForTokenModel?> GetUserPermissionsAsync(Guid id)
	{
		return await _usersRepository.GetUserPermissionsAsync(id);
	}
	public async Task<List<Permission>> GetPermissionsForUserAsync(Guid userId)
	{
		return await _repository.GetPermissionsForUserAsync(userId);
	}
	public async Task<List<Role>> GetRoleForUserAsync(Guid userId)
	{
		return await _repository.GetRoleForUserAsync(userId);
	}
}
