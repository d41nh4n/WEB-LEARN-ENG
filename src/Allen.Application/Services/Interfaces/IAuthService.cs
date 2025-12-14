using Google.Apis.Auth;

namespace Allen.Application;
public interface IAuthService
{
	Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(string idToken);
	Task<User> GetOrCreateUserAsync(GoogleJsonWebSignature.Payload payload);
	Task<string> GenerateAccessToken(User user);
	string GenerateRefreshToken();
	Task<UserInfoModel> GetUserByRefreshTokenAsync(Guid id);
	ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    Task<OperationResult> ForgotPasswordAsync(ForgotPasswordModel model);
    Task<OperationResult> ResetPasswordAsync(ResetPasswordModel model);
    Task<OperationResult> ChangePasswordAsync(ChangePasswordModel model);
}
