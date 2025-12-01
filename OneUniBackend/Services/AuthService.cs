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
using Google.Apis.Auth;
using OneUniBackend.DTOs.Common;
using OneUniBackend.Enums;
using OneUniBackend.Data;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;


namespace OneUniBackend.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IPasswordService _passwordService;
    private readonly JWTSettings _jwtSettings;
    private readonly IGoogleOAuthService _googleOAuthService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUnitOfWork unitOfWork, ITokenService tokenService, IPasswordService passwordService, IOptions<JWTSettings> jwtSettings, IGoogleOAuthService googleOAuthService, ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _passwordService = passwordService;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
        _googleOAuthService = googleOAuthService;
    }
    public async Task<Result<AuthResponseDTO<UserDTO>>> RegisterAsync(SignUpRequestDTO request, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            string refreshToken, accessToken;
            var checkUser = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken: cancellationToken);
            if (checkUser != null)
            {
                return Result<AuthResponseDTO<UserDTO>>.Failure("USER_ALREADY_EXISTS");
            }
            var user = new User
            {
                UserId = Guid.NewGuid(),
                FullName = request.FullName,
                Email = request.Email,
                PasswordHash = _passwordService.HashPassword(request.Password),
                Role = request.Role,
                CreatedAt = DateTime.UtcNow,

            };
            // user creation
            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            // token generation
            accessToken = _tokenService.GenerateAccessToken(user);
            refreshToken = _tokenService.GenerateRefreshToken();
            string refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);

            // refresh Token save operation
            Result<string> refreshTokenSave = await _tokenService.SaveRefreshTokenAsync(user.UserId, refreshTokenHash, cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            var authResponse = new AuthResponseDTO<UserDTO>
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
            return Result<AuthResponseDTO<UserDTO>>.Success(authResponse);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<AuthResponseDTO<UserDTO>>.Failure("USER_REGISTRATION_FAILED");
        }
    }
    public async Task<Result<AuthResponseDTO<UserDTO>>> LoginAsync(LoginRequestDTO request, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email, cancellationToken: cancellationToken);
            if (user == null || user.PasswordHash == null || !_passwordService.VerifyPassword(request.Password, user.PasswordHash))
            {
                return Result<AuthResponseDTO<UserDTO>>.Failure("INVALID_CREDENTIALS");
            }
            string accessToken;
            string refreshToken;
            string refreshTokenHash;
            accessToken = _tokenService.GenerateAccessToken(user);
            refreshToken = _tokenService.GenerateRefreshToken();
            refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);
            user.LastLogin = DateTime.UtcNow;
            Result<string> saveRefreshTokenResult = await _tokenService.SaveRefreshTokenAsync(user.UserId, refreshTokenHash, cancellationToken);
            if (saveRefreshTokenResult.IsSuccess == false)
            {
                return Result<AuthResponseDTO<UserDTO>>.Failure("REFRESH_TOKEN_SAVE_FAILED");
            }
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            var authResponse = new AuthResponseDTO<UserDTO>
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
            return Result<AuthResponseDTO<UserDTO>>.Success(authResponse);
        }
        catch (InvalidOperationException)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<AuthResponseDTO<UserDTO>>.Failure("TOKEN_GENERATION_FAILED");
        }
    }

    public async Task<Result<AuthResponseDTO<UserDTO>>> GoogleLoginAsync(GoogleUserInfo googleuserObject, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            User? userFromGoogleID = await _googleOAuthService.GetUserByGoogleIDAsync(googleuserObject.GoogleUserId, cancellationToken);
            User? userFromEmail = await _unitOfWork.Users.GetByEmailAsync(googleuserObject.UserEmail, cancellationToken);
            if (userFromGoogleID == null || userFromEmail == null || userFromEmail != userFromGoogleID || userFromGoogleID.Email != userFromEmail.Email)
            {
                return Result<AuthResponseDTO<UserDTO>>.Failure("INVALID_SIGNUP_REQUEST");
            }
            string refreshToken, accessToken, refreshTokenHash;
            accessToken = _tokenService.GenerateAccessToken(userFromGoogleID);
            refreshToken = _tokenService.GenerateRefreshToken();
            refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);
            userFromGoogleID.LastLogin = DateTime.UtcNow;
            Result<string> saveRefreshTokenResult = await _tokenService.SaveRefreshTokenAsync(userFromGoogleID.UserId, refreshTokenHash, cancellationToken);
            if (!saveRefreshTokenResult.IsSuccess)
            {
                return Result<AuthResponseDTO<UserDTO>>.Failure("REFRESH_TOKEN_SAVE_FAILED");
            }
            await _unitOfWork.CommitTransactionAsync();
            return Result<AuthResponseDTO<UserDTO>>.Success(new AuthResponseDTO<UserDTO>
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiresInMinutes),
                User = new UserDTO
                {
                    Id = userFromGoogleID.UserId,
                    Email = userFromGoogleID.Email,
                    Role = userFromGoogleID.Role
                }
            });
        }
        catch (InvalidOperationException)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<AuthResponseDTO<UserDTO>>.Failure("TOKEN_GENERATION_FAILED");
        }

    }

    public async Task<Result<AuthResponseDTO<GoogleUserInfo>>> TempGoogleSignUpAsync(GoogleUserInfo googleuUserObject, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            User? existingUserByEmail = await _unitOfWork.Users.GetByEmailAsync(googleuUserObject.UserEmail, cancellationToken);
            if (existingUserByEmail != null)
            {
                return Result<AuthResponseDTO<GoogleUserInfo>>.Failure("USER_ALREADY_EXISTS");
            }
            string tempAccessToken = _tokenService.GenerateTemporaryAccessToken(googleuUserObject);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            var authResponse = new AuthResponseDTO<GoogleUserInfo>
            {
                AccessToken = tempAccessToken,
                RefreshToken = string.Empty,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiresInMinutes),
                User = googleuUserObject
            };
            return Result<AuthResponseDTO<GoogleUserInfo>>.Success(authResponse);
        }
        catch (InvalidOperationException)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<AuthResponseDTO<GoogleUserInfo>>.Failure("TOKEN_GENERATION_FAILED");
        }
    }

    public async Task<Result<AuthResponseDTO<UserDTO>>> CompleteGoogleSignupAsync(CompleteGoogleSignUpRequestDTO googleUserObject, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            User? findUserByEmail = await _unitOfWork.Users.GetByEmailAsync(googleUserObject.UserEmail, cancellationToken);
            User? findUserByGoogleID = await _googleOAuthService.GetUserByGoogleIDAsync(googleUserObject.GoogleUserId);
            if (findUserByGoogleID != null || findUserByGoogleID != null)
            {
                return Result<AuthResponseDTO<UserDTO>>.Failure("USER_ALREADY_EXISTS");

            }
            User newUser = new User
            {
                UserId = Guid.NewGuid(),
                FullName = googleUserObject.UserName,
                Email = googleUserObject.UserEmail,
                Role = googleUserObject.Role,
                CreatedAt = DateTime.UtcNow,
                LastLogin = DateTime.UtcNow,
            };
            await _unitOfWork.Users.AddAsync(newUser);
            UserLogin newUserLogin = new UserLogin
            {
                UserId = newUser.UserId,
                Providerkey = googleUserObject.GoogleUserId,
                Providerdisplayname = "google",
                Loginprovider = "google"
            };
            await _unitOfWork.UserExternalLoginRepository.AddAsync(newUserLogin);
            string accessToken, refreshToken, refreshTokenHash;
            accessToken =  _tokenService.GenerateAccessToken(newUser);
            refreshToken = _tokenService.GenerateRefreshToken();
            refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);
            Result<string> saveRefreshToken = await _tokenService.SaveRefreshTokenAsync(newUser.UserId, refreshTokenHash, cancellationToken);
            await _unitOfWork.CommitTransactionAsync();
            
            var authResponse = new AuthResponseDTO<UserDTO>
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpiresInMinutes),
                User = new UserDTO
                {
                    Id = newUser.UserId,
                    Email = newUser.Email,
                    Role = newUser.Role,
                }
            };
            return Result<AuthResponseDTO<UserDTO>>.Success(authResponse);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<AuthResponseDTO<UserDTO>>.Failure("TOKEN_GENERATION_FAILED");
        }
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

    public async Task<Result<AuthResponseDTO<UserDTO>>> RefreshTokenAsync(RefreshTokenRequestDTO request, CancellationToken cancellationToken = default)
    {
        var refreshTokenHash = _tokenService.HashRefreshToken(request.RefreshToken);
        Result<User?> validateResult = await _tokenService.ValidateRefreshTokenAsync(refreshTokenHash, cancellationToken);

        if (validateResult.IsSuccess == false || validateResult.Data == null)
        {
            return Result<AuthResponseDTO<UserDTO>>.Failure("INVALID_REFRESH_TOKEN");
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
                return Result<AuthResponseDTO<UserDTO>>.Failure("REFRESH_TOKEN_SAVE_FAILED");
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            var authResponse = new AuthResponseDTO<UserDTO>
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
            return Result<AuthResponseDTO<UserDTO>>.Success(authResponse);
        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return Result<AuthResponseDTO<UserDTO>>.Failure("TOKEN_REFRESH_FAILED");
        }
    }

    // public async Task<Result<AuthResponseDTO>> GoogleExternalLogin(GoogleLoginRequestDTO request, CancellationToken cancellationToken)
    // {
    //     await _unitOfWork.BeginTransactionAsync(cancellationToken);
    //     User googleUser = null;
    //     try
    //     {
    //         GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);
    //         if (payload == null || payload.Email == null)
    //         {
    //             return Result<AuthResponseDTO>.Failure("INVALID_GOOGLE_TOKEN");
    //         }
    //         // DB Context should not be used directly. We Use UnitOfWork pattern for that.

    //         // UseCase: Existing Google User logging in
    //         googleUser = await _googleOAuthService.GetUserByGoogleIDAsync(payload.GoogleID);


    //         {// Step 2: Check if user exists
    //             var user = await _unitOfWork.Users.GetByEmailAsync(payload.Email, cancellationToken);
    //             // this endpoint should handle both login and registration flows as google uses the same token for both
    //             // Step 3: If user does not exist, create new user

    //             {
    //                 if (user == null)
    //                 {
    //                     user = new User
    //                     {
    //                         UserId = Guid.NewGuid(),
    //                         Email = payload.Email,
    //                         FullName = payload.Name,
    //                         IsActive = true,
    //                         IsVerified = true,
    //                         CreatedAt = DateTime.UtcNow,
    //                     };
    //                     // Use UnitOfWork pattern to add and save user
    //                     await _unitOfWork.Users.AddAsync(user, cancellationToken);
    //                     await _unitOfWork.SaveChangesAsync(cancellationToken);
    //                 }
    //             }
    //             // Step 3B: User exists, update last login
    //             {
    //                 if (user != null)
    //                 {
    //                     user.LastLogin = DateTime.UtcNow;
    //                     await _unitOfWork.SaveChangesAsync(cancellationToken);
    //                 }
    //             }

    //             // Step 4: Generate Tokens
    //             var tokens = await GenerateTokensForUserAsync(user, cancellationToken);

    //             // Step 5: Build response
    //             return Result<AuthResponseDTO>.Success(new AuthResponseDTO
    //             {
    //                 AccessToken = tokens.AccessToken,
    //                 RefreshToken = tokens.RefreshToken,
    //                 ExpiresAt = tokens.ExpiresAt,
    //                 User = new UserDTO
    //                 {
    //                     Id = user.UserId,
    //                     Email = user.Email,
    //                     Role = user.Role
    //                 }
    //             });
    //         }
    //     }
    //     catch (Exception ex)
    //     {
    //         _logger.LogError(ex, "GOOGLE_LOGIN_FAILED");
    //         return Result<AuthResponseDTO>.Failure("GOOGLE_LOGIN_FAILED");
    //     }

    // }


    private async Task<AuthResponseDTO<UserDTO>> GenerateTokensForUserAsync(User user, CancellationToken cancellationToken)
    {
        try
        {
            // 1. Generate access + refresh tokens
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshTokenHash = _tokenService.HashRefreshToken(refreshToken);

            // 2. Save refresh token in DB
            await _tokenService.SaveRefreshTokenAsync(user.UserId, refreshTokenHash, cancellationToken);

            // 3. Extract expiration from access token
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(accessToken);
            var expiresAt = token.ValidTo;

            // 4. Return full DTO
            return new AuthResponseDTO<UserDTO>
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine("TOKEN_GENERATION_ERROR: " + ex.Message);
            throw;
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
