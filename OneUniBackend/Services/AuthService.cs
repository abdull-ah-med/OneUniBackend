using System;
using Microsoft.Extensions.Options;
using OneUni.Common;
using OneUni.DTOs.Auth;
using OneUni.DTOs.User;
using OneUni.Interfaces.Repositories;
using OneUni.Interfaces.Services;
using OneUni.Configuration;
using OneUni.Entities;
using Microsoft.EntityFrameworkCore;


namespace OneUni.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IPasswordService _passwordService;
    private readonly JWTSettings _jwtSettings;

    public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IPasswordService passwordService, IOptions<JWTSettings> jwtSettings)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _passwordService = passwordService;
        _jwtSettings = jwtSettings.Value;
    }
    public async Task<Result<AuthResponseDTO>> RegisterAsync(SignUpRequestDTO request, CancellationToken cancellationToken = default)
    {
        string refreshToken, accessToken;
        var checkUser = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken: cancellationToken);
        if (checkUser != null)
        {
            return Result<AuthResponseDTO>.Failure("USER_ALREADY_EXISTS");
        }
        var user = new User
        {
            Email = request.Email,
            PasswordHash = _passwordService.HashPassword(request.Password),
            Role = request.Role,
            CreatedAt = DateTime.UtcNow,

        };
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // user creation
            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // token generation
            accessToken = _tokenService.GenerateAccessToken(user);
            refreshToken = _tokenService.GenerateRefreshToken();
            string refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);

            // refresh Token entity creation
            var userRefreshToken = new UserRefreshToken
            {
                UserId = user.UserId,
                TokenHash = refreshTokenHash,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpiresInDays),
                IsRevoked = false
            };
            await _unitOfWork.UserRefreshTokens.AddAsync(userRefreshToken, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            var authResponse = new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiresInMinutes),
                User = new UserDTO
                {
                    Id = user.UserId,
                    Email = user.Email,
                    Role = user.Role,
                }
            };
            return Result<AuthResponseDTO>.Success(authResponse);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<AuthResponseDTO>.Failure("USER_REGISTRATION_FAILED");
        }
    }
    public async Task<Result<AuthResponseDTO>> LoginAsync(LoginRequestDTO request, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken: cancellationToken);
        if (user == null || !_passwordService.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result<AuthResponseDTO>.Failure("INVALID_CREDENTIALS");
        }
        string accessToken;
        string refreshToken;
        string refreshTokenHash;
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            accessToken = _tokenService.GenerateAccessToken(user);
            refreshToken = _tokenService.GenerateRefreshToken();
            refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);
        }
        catch (InvalidOperationException)
        {
            return Result<AuthResponseDTO>.Failure("TOKEN_GENERATION_FAILED");
        }
        user.LastLogin = DateTime.UtcNow;
        Result<string> saveRefreshTokenResult = await _tokenService.SaveRefreshTokenAsync(user.UserId, refreshTokenHash, cancellationToken);
        if (saveRefreshTokenResult.IsSuccess == false)
        {
            return Result<AuthResponseDTO>.Failure("REFRESH_TOKEN_SAVE_FAILED");
        }
        var authResponse = new AuthResponseDTO
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiresInMinutes),
            User = new UserDTO
            {
                Id = user.UserId,
                Email = user.Email,
                Role = user.Role,
            }
        };
        await _unitOfWork.CommitTransactionAsync(cancellationToken);
        return Result<AuthResponseDTO>.Success(authResponse);
    }
    public Task<Result<AuthResponseDTO>> RefreshTokenAsync(RefreshTokenRequestDTO request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    public Task<Result<UserDTO>> GetCurrentUserAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    public Task<Result> LogoutAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    public Task<Result> RevokeTokenAsync(RevokeTokenRequestDTO request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    public Task<Result<bool>> ChangePasswordAsync(ChangePasswordRequestDTO request, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    public Task<Result> RevokeAllRefreshTokensAsync(CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    public Task<Result<bool>> VerifyEmailAsync(string token, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    public Task<Result<bool>> RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
    public Task<Result> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
