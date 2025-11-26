using System;
using Microsoft.Extensions.Options;
using OneUniBackend.Common;
using OneUniBackend.DTOs.Auth;
using OneUniBackend.DTOs.User;
using OneUniBackend.Interfaces.Repositories;
using OneUniBackend.Interfaces.Services;
using OneUniBackend.Configuration;
using OneUniBackend.Entities;
using Microsoft.EntityFrameworkCore;


namespace OneUniBackend.Infrastructure.Services;

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
            FullName = request.FullName,
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
            var newUser = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken: cancellationToken);
            // token generation
            accessToken = _tokenService.GenerateAccessToken(user);
            refreshToken = _tokenService.GenerateRefreshToken();
            string refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);

            // refresh Token entity creation
            Result<string> refreshTokenSave = await _tokenService.SaveRefreshTokenAsync(newUser.UserId, refreshTokenHash);
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
    public async Task<Result<UserDTO>> GetCurrentUserAsync(Guid userID, CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userID, cancellationToken);
        if (user == null)
        {
            return Result<UserDTO>.Failure("USER_NOT_FOUND");
        }
        var userDTO = new UserDTO
        {
            Id = user.UserId,
            Email = user.Email,
            Role = user.Role,
        };
        return Result<UserDTO>.Success(userDTO);
    }
    public async Task<Result> LogoutAsync(LogoutDTO request, CancellationToken cancellationToken = default)
    {
        var refreshTokenHash = _tokenService.HashRefreshToken(request.RefreshToken);
        Result revokeResult = await _tokenService.RevokeRefreshTokenAsync(refreshTokenHash, cancellationToken);
        return revokeResult;
    }
    public async Task<Result<bool>> ChangePasswordAsync(Guid userID, ChangePasswordRequestDTO request, CancellationToken cancellationToken = default)
    {

        var refreshTokenHash = _tokenService.HashRefreshToken(request.refreshToken);
        Result<User?> validateResult = await _tokenService.ValidateRefreshTokenAsync(refreshTokenHash, cancellationToken);

        if (validateResult.IsSuccess == false || validateResult.Data == null || validateResult.Data.UserId != userID)
        {
            return Result<bool>.Failure("INVALID_REFRESH_TOKEN");
        }
        var user = validateResult.Data;
        if (!_passwordService.VerifyPassword(request.CurrentPassword, user.PasswordHash))
        {
            return Result<bool>.Failure("INVALID_CURRENT_PASSWORD");
        }
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            user.PasswordHash = _passwordService.HashPassword(request.NewPassword);
            await _tokenService.RevokeAllRefreshTokensForUserAsync(user.UserId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<bool>.Failure("PASSWORD_CHANGE_FAILED");
        }
        return Result<bool>.Success(true);
    }

    public async Task<Result<AuthResponseDTO>> RefreshTokenAsync(RefreshTokenRequestDTO request, CancellationToken cancellationToken = default)
    {
        var refreshTokenHash = _tokenService.HashRefreshToken(request.RefreshToken);
        Result<User?> validateResult = await _tokenService.ValidateRefreshTokenAsync(refreshTokenHash, cancellationToken);

        if (validateResult.IsSuccess == false || validateResult.Data == null)
        {
            return Result<AuthResponseDTO>.Failure("INVALID_REFRESH_TOKEN");
        }

        var user = validateResult.Data;
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // Generate new tokens
            var accessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var newRefreshTokenHash = _tokenService.HashRefreshToken(newRefreshToken);

            // Revoke old refresh token
            await _tokenService.RevokeRefreshTokenAsync(refreshTokenHash, cancellationToken);

            // Save new refresh token
            Result<string> saveResult = await _tokenService.SaveRefreshTokenAsync(user.UserId, newRefreshTokenHash, cancellationToken);
            if (saveResult.IsSuccess == false)
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                return Result<AuthResponseDTO>.Failure("REFRESH_TOKEN_SAVE_FAILED");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var authResponse = new AuthResponseDTO
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken,
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
            return Result<AuthResponseDTO>.Failure("TOKEN_REFRESH_FAILED");
        }
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
