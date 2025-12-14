using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using Google.Apis.Auth;
using Microsoft.IdentityModel.Tokens;

namespace Allen.Application;

[RegisterService(typeof(IAuthService))]
public class AuthService(
    IUsersService _usersService,
    IRolesService _rolesService,
    IAppConfiguration _configuration,
    IUnitOfWork _unitOfWork,
    IEmailService _emailService,
    IMapper _mapper) : IAuthService
{

    public async Task<GoogleJsonWebSignature.Payload> VerifyGoogleTokenAsync(string idToken)
    {
        var jwtSettings = _configuration.GetGoogleSettings();

        var settings = new GoogleJsonWebSignature.ValidationSettings
        {
            Audience = [jwtSettings.ClientId]
        };

        return await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
    }
    public async Task<User> GetOrCreateUserAsync(GoogleJsonWebSignature.Payload payload)
    {
        var existingUser = await _unitOfWork.Repository<UserEntity>().GetByConditionAsync(x => x.Email == payload.Email);
        if (existingUser != null)
            return _mapper.Map<User>(existingUser);

        // // Add new user to database
        var insertedUser = await _unitOfWork.Repository<UserEntity>().AddAsync(
            new UserEntity
            {
                Id = Guid.NewGuid(),
                Email = payload.Email,
                Name = payload.Name,
                Picture = payload.Picture,
                IsDeleted = false,
				Password = BCrypt.Net.BCrypt.HashPassword(payload.Email),
            });

        var role = await _unitOfWork.Repository<RoleEntity>().GetByConditionAsync(x => x.Name == RoleType.User);
        if (role == null)
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(Role), RoleType.User));

        var insertedUserRole = await _unitOfWork.Repository<UserRoleEntity>().AddAsync(new UserRoleEntity
        {
            Id = Guid.NewGuid(),
            UserId = insertedUser.Id,
            RoleId = role.Id
        });

        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<User>(insertedUser);
    }
    public async Task<string> GenerateAccessToken(User user)
    {
        var jwtSettings = _configuration.GetJwtSetting();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var roles = await _rolesService.GetRoleForUserAsync(user.Id);

        var claims = new List<Claim>
        {
            new Claim("Id", user.Id.ToString() ?? ""),
            new Claim(ClaimTypes.Name, user.Name ?? ""),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim("Picture", user.Picture ?? "")
        };
        claims.AddRange(roles!.Select(role => new Claim(ClaimTypes.Role, role.Name!)));

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_configuration.GetJwtSetting().AccessTokenExpiration),
            signingCredentials: creds)
        ;

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        var jwtSettings = _configuration.GetJwtSetting();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            expires: DateTime.UtcNow.AddDays(_configuration.GetJwtSetting().AccessTokenExpiration),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    public async Task<UserInfoModel> GetUserByRefreshTokenAsync(Guid id)
    {
        var user = await _usersService.GetByIdAsync(id);
        if (user == null)
        {
            throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(User), id));
        }
        return user;
    }
    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        var jwtSettings = _configuration.GetJwtSetting();
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = false
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

        if (validatedToken is not JwtSecurityToken jwtToken || !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            throw new SecurityTokenException("Invalid token");

        return principal;
    }
    public async Task<OperationResult> ForgotPasswordAsync(ForgotPasswordModel model)
    {
        var user = await _unitOfWork.Repository<UserEntity>()
            .GetByConditionAsync(x => x.Email == model.Email);
        if (user == null) throw new NotFoundException(ErrorMessageBase.Format(ErrorMessageBase.NotFound, nameof(User), model.Email ?? ""));

        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        await _unitOfWork.Repository<PasswordResetTokenEntity>().AddAsync(new PasswordResetTokenEntity
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = token,
            Expiration = DateTime.UtcNow.AddMinutes(15)
        });

        if (!await _unitOfWork.SaveChangesAsync())
        {
            throw new Exception(ErrorMessageBase.Format(ErrorMessageBase.CreateFailure, nameof(ForgotPasswordModel)));
        }

        var urlSettings = _configuration.GetUrlSettings();
        var link = $"{urlSettings.ClientUrl}/reset-password?token={token}";
        var emailContent = new EmailContent
        {
            To = user.Email,
            Subject = "Reset your password",
            Body = $"Click <a href='{link}'>here</a> to reset your password."
        };
        await _emailService.SendMailAsync(emailContent);
        return OperationResult.SuccessResult();
    }

    public async Task<OperationResult> ResetPasswordAsync(ResetPasswordModel model)
    {
        try
        {
            await _unitOfWork.ExecuteWithTransactionAsync(async () =>
            {
                var resetToken = await _unitOfWork.Repository<PasswordResetTokenEntity>()
            .GetByConditionAsync(x => x.Token == model.Token && x.Expiration > DateTime.UtcNow && !x.IsUsed);

                if (resetToken == null) throw new BadRequestException("Token invalid or expired");

                var user = await _unitOfWork.Repository<UserEntity>()
                     .GetByIdAsync(resetToken.UserId);

                user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                resetToken.IsUsed = true;

                var result = await _unitOfWork.SaveChangesAsync();
                if (!result)
                {
                    throw new Exception(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(User)));
                }
            });
            return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(ResetPasswordModel)));
        }
        catch (Exception ex)
        {
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(ResetPasswordModel), ex.Message));
        }
    }

    public async Task<OperationResult> ChangePasswordAsync(ChangePasswordModel model)
    {
        var user = await _unitOfWork.Repository<UserEntity>().GetByIdAsync(model.UserId);

        if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.Password))
            throw new BadRequestException("Wrong current password");

        user.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
        if (!await _unitOfWork.SaveChangesAsync())
        {
            return OperationResult.Failure(ErrorMessageBase.Format(ErrorMessageBase.UpdateFailure, nameof(ChangePasswordModel)));
        }
        return OperationResult.SuccessResult(ErrorMessageBase.Format(ErrorMessageBase.UpdatedSuccess, nameof(ChangePasswordModel)));
    }
}
