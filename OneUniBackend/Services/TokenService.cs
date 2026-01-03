using System;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using OneUniBackend.Interfaces.Services;
using OneUniBackend.Entities;
using OneUniBackend.Interfaces.Repositories;
using OneUniBackend.Common;
using OneUniBackend.Configuration;
using Microsoft.Extensions.Options;
using OneUniBackend.DTOs.Auth;

namespace OneUniBackend.Services;

public class TokenService : ITokenService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly TokenValidationParameters _validationParameters;
    private readonly JWTSettings _jwtSettings;

    public TokenService(TokenValidationParameters validationParameters, IUnitOfWork unitOfWork, IOptions<JWTSettings> jwtSettings)
    {
        _unitOfWork = unitOfWork;
        _jwtSettings = jwtSettings.Value;
        _validationParameters = validationParameters;
    }
    public string GenerateAccessToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var secret = _jwtSettings.SecretKey;
        if (string.IsNullOrEmpty(secret))
            throw new InvalidOperationException("JWT_SECRET_KEY is not configured.");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.ToString())
        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwtSettings.Issuer,
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiresInMinutes),

            SigningCredentials = creds
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    public string GenerateTemporaryAccessToken(string code, GoogleUserInfo googleUserInfo)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var secret = _jwtSettings.SecretKey;
        if (string.IsNullOrEmpty(secret))
            throw new InvalidOperationException("JWT_SECRET_KEY is not configured.");

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, code),
            new Claim(ClaimTypes.Name, googleUserInfo.GoogleUserId.ToString()),
            new Claim(ClaimTypes.Email, googleUserInfo.UserEmail),
            new Claim(ClaimTypes.Role, googleUserInfo.UserName),
            new Claim("isEmailVerified", googleUserInfo.isEmailVerified.ToString()) 
        };
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _jwtSettings.Issuer,
            Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiresInMinutes),

            SigningCredentials = creds
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    public string GenerateRefreshToken()
    {
        var randomBytes = RandomNumberGenerator.GetBytes(32);
        return Convert.ToHexString(randomBytes);
    }
    public string HashRefreshToken(string refreshToken)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(refreshToken);
        var hashBytes = sha256.ComputeHash(bytes);
        return Convert.ToHexString(hashBytes);
    }
    public async Task<Result<string>> SaveRefreshTokenAsync(Guid userId, string refreshTokenHash, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                return Result<string>.Failure("USER_NOT_FOUND");
            }
            var newRefreshToken = new UserRefreshToken
            {
                UserId = user.UserId,
                TokenHash = refreshTokenHash,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiresInDays),
                CreatedAt = DateTime.UtcNow,
                IsRevoked = false
            };
            await _unitOfWork.UserRefreshTokens.AddAsync(newRefreshToken, cancellationToken);

            return Result<string>.Success(refreshTokenHash);
        }
        catch (Exception)
        {
            return Result<string>.Failure("REFRESH_TOKEN_SAVE_FAILED");
        }

    }
    public async Task<Result<User?>> ValidateRefreshTokenAsync(string refreshTokenHash, CancellationToken cancellationToken = default)
    {

        var userRefreshToken = await _unitOfWork.UserRefreshTokens.GetByRefreshTokenAsync(refreshTokenHash, cancellationToken);
        if (userRefreshToken == null || userRefreshToken.IsRevoked == true || userRefreshToken.ExpiresAt <= DateTime.UtcNow)
        {
            return Result<User?>.Failure("INVALID_REFRESH_TOKEN");
        }
        var user = await _unitOfWork.Users.GetByIdAsync(userRefreshToken.UserId, cancellationToken);
        if (user == null)
        {
            return Result<User?>.Failure("USER_NOT_FOUND");
        }
        return Result<User?>.Success(user);
    }
    public async Task<Result> RevokeRefreshTokenAsync(string refreshTokenHash, CancellationToken cancellationToken = default)
    {
        bool result = await _unitOfWork.UserRefreshTokens.RevokeRefreshTokenAsync(refreshTokenHash, cancellationToken);
        if (result)
        {
            return Result.Success();
        }
        return Result.Failure("REFRESH_TOKEN_REVOKE_FAILED");

    }
    public async Task<Result> RevokeAllRefreshTokensForUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        bool result = await _unitOfWork.UserRefreshTokens.RevokeAllUserRefreshTokensAsync(userId, cancellationToken);
        if (result)
        {
            return Result.Success();
        }
        else
        {
            return Result.Failure("REFRESH_TOKENS_REVOKE_FAILED");
        }
    }
    public async Task RemoveExpiredRefreshTokensAsync(CancellationToken cancellationToken = default)
    {
        bool result = await _unitOfWork.UserRefreshTokens.RemoveExpiredRefreshTokensAsync(cancellationToken);
    }

    public Result<CompleteGoogleSignUpRequestDTO> ValidateTemporaryAccessToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secret = _jwtSettings.SecretKey;
            if (string.IsNullOrEmpty(secret))
                return Result<CompleteGoogleSignUpRequestDTO>.Failure("JWT_SECRET_KEY_NOT_CONFIGURED");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var validationParameters = _validationParameters.Clone();
            validationParameters.ValidateLifetime = true;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);

            var code = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var emailClaim = principal.FindFirst(ClaimTypes.Email)?.Value;
            var userNameClaim = principal.FindFirst(ClaimTypes.Role)?.Value;
            var GoogleUserIdClaim = principal.FindFirst(ClaimTypes.Name)?.Value; // Note: Using Role claim for UserName in temporary token
            var isEmailVerified = bool.Parse(principal.FindFirst("isEmailVerified")!.Value);


            if (string.IsNullOrEmpty(GoogleUserIdClaim) || string.IsNullOrEmpty(emailClaim))
            {
                return Result<CompleteGoogleSignUpRequestDTO>.Failure("INVALID_TOKEN_CLAIMS");
            }

            var completeGoogleUserInfo = new CompleteGoogleSignUpRequestDTO
            {
                GoogleUserId = GoogleUserIdClaim,
                UserEmail = emailClaim,
                UserName = userNameClaim ?? string.Empty,
                Code = code,
                IsEmailVerified = isEmailVerified
            };

            return Result<CompleteGoogleSignUpRequestDTO>.Success(completeGoogleUserInfo);
        }
        catch (SecurityTokenExpiredException)
        {
            return Result<CompleteGoogleSignUpRequestDTO>.Failure("TOKEN_EXPIRED");
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            return Result<CompleteGoogleSignUpRequestDTO>.Failure("INVALID_TOKEN_SIGNATURE");
        }
        catch (Exception)
        {
            return Result<CompleteGoogleSignUpRequestDTO>.Failure("TOKEN_VALIDATION_FAILED");
        }
    }
}
