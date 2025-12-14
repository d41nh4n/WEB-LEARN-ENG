namespace Allen.API.Controllers;

[Route("auth")]
public class AuthController(
	IUsersService _usersService,
	IAuthService _authService,
	IAppConfiguration _appConfiguration,
	IMapper _mapper) : BaseApiController
{
	[HttpPost("signin-google")]
	public async Task<TokenResponse> SignInGoogle([FromBody] LoginGoogleModel model)
	{
		var payload = await _authService.VerifyGoogleTokenAsync(model.IdToken);
		var user = await _authService.GetOrCreateUserAsync(payload);
		var accessToken = await _authService.GenerateAccessToken(user);
		var refreshToken = _authService.GenerateRefreshToken();
		return new TokenResponse
		{
			AccessToken = accessToken,
			ExpiresIn = _appConfiguration.GetJwtSetting().AccessTokenExpiration * 60
		};
	}

	[HttpPost("login")]
	[ValidateModel]
	public async Task<TokenResponse> Login([FromBody] LoginModel model)
	{
		var user = await _usersService.LoginAsync(model);
		var accessToken = await _authService.GenerateAccessToken(user);
		var refreshToken = _authService.GenerateRefreshToken();

		await _usersService.UpdateRefreshTokenAsync(user.Id, refreshToken);

		return new TokenResponse
		{
			AccessToken = accessToken,
			ExpiresIn = _appConfiguration.GetJwtSetting().AccessTokenExpiration * 60
		};
	}
	[HttpPost("refresh-token")]
	public async Task<TokenResponse> RefreshToken(RefreshTokenModel model)
	{
		var principal = _authService.GetPrincipalFromExpiredToken(model.Token);
		var userId = principal.Claims.FirstOrDefault(x => x.Type == "Id")?.Value;

		var userWithRefreshToken = await _usersService.GetByIdForRefreshTokenAsync(Guid.Parse(userId ?? ""));

		var user = _mapper.Map<User>(userWithRefreshToken);
		var newAccessToken = await _authService.GenerateAccessToken(user);
		var newRefreshToken = _authService.GenerateRefreshToken();

		await _usersService.UpdateRefreshTokenAsync(user.Id, newRefreshToken);

		return new TokenResponse
		{
			AccessToken = newAccessToken,
			ExpiresIn = 2 * 60
		};
	}
	[HttpPost("logout")]
	[Authorize]
    public async Task<OperationResult> Logout(RefreshTokenModel model)
	{
		var principal = _authService.GetPrincipalFromExpiredToken(model.Token);
		var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
        var removeRefreshToken = "";

		await _usersService.UpdateRefreshTokenAsync(userId, removeRefreshToken);

		return new OperationResult
		{
			Success = true,
			Message = "Logout successful"
		};
	}
	[HttpPost("register")]
	[ValidateModel]
	public async Task<OperationResult> CreateUser([FromBody] RegisterModel model)
	{
		return await _usersService.CreateAsync(model);
	}
    [HttpPost("forgot-password")]
	[ValidateModel]
    public async Task<OperationResult> ForgotPassword([FromBody] ForgotPasswordModel model)
    {
        return await _authService.ForgotPasswordAsync(model);
    }
    [HttpPost("reset-password")]
    [ValidateModel]
    public async Task<OperationResult> ResetPassword([FromBody] ResetPasswordModel model)
	{
		return await _authService.ResetPasswordAsync(model);
	}
    [HttpPost("change-password")]
	[Authorize]
    [ValidateModel]
    public async Task<OperationResult> ChangePassword([FromBody] ChangePasswordModel model)
    {
		var userId = HttpContextHelper.GetCurrentUserId(HttpContext);
		model.UserId = userId;
        return await _authService.ChangePasswordAsync(model);
    }
}
