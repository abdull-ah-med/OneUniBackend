using OneUniBackend.Common;
using OneUniBackend.DTOs.Auth;
using OneUniBackend.DTOs.User;

namespace OneUniBackend.Interfaces.Services;

public interface IAuthService
{
    Task<Result<AuthResponseDTO>> RegisterAsync(SignUpRequestDTO request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponseDTO>> LoginAsync(LoginRequestDTO request, CancellationToken cancellationToken = default);
    Task<Result<UserDTO>> GetCurrentUserAsync(Guid userID, CancellationToken cancellationToken = default);
    Task<Result> LogoutAsync(LogoutDTO request, CancellationToken cancellationToken = default);
    Task<Result<bool>> ChangePasswordAsync(Guid userID, ChangePasswordRequestDTO request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponseDTO>> RefreshTokenAsync(RefreshTokenRequestDTO request, CancellationToken cancellationToken = default);
    Task<Result<bool>> VerifyEmailAsync(string token, CancellationToken cancellationToken = default);
    Task<Result<bool>> RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default);
    Task<Result> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default);
}

