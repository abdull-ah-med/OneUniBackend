using OneUniBackend.Common;
using OneUniBackend.DTOs.Auth;
using OneUniBackend.DTOs.User;
using OneUniBackend.Enums;

namespace OneUniBackend.Interfaces.Services;

public interface IAuthService
{
    Task<Result<AuthResponseDTO<UserDTO>>> RegisterAsync(SignUpRequestDTO request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponseDTO<UserDTO>>> LoginAsync(LoginRequestDTO request, CancellationToken cancellationToken = default);
    Task<Result<UserDTO>> GetCurrentUserAsync(Guid userID, CancellationToken cancellationToken = default);
    Task<Result> LogoutAsync(LogoutDTO request, CancellationToken cancellationToken = default);
    Task<Result<bool>> ChangePasswordAsync(Guid userID, ChangePasswordRequestDTO request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponseDTO<UserDTO>>> RefreshTokenAsync(RefreshTokenRequestDTO request, CancellationToken cancellationToken = default);
    Task<Result<bool>> VerifyEmailAsync(string token, CancellationToken cancellationToken = default);
    Task<Result<bool>> RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default);
    Task<Result> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default);
    Task<Result<AuthResponseDTO<UserDTO>>> GoogleLoginAsync(GoogleUserInfo googleuserObject, CancellationToken cancellationToken);
    Task<Result<AuthResponseDTO<GoogleUserInfo>>> TempGoogleSignUpAsync(string code, GoogleUserInfo googleUserObject, CancellationToken cancellationToken);
    Task<Result<AuthResponseDTO<UserDTO>>> CompleteGoogleSignupAsync(UserRoleDTO userRole, string temporaryAccessToken, CancellationToken cancellationToken = default);    
}

