using OneUni.Common;
using OneUni.DTOs.Auth;
using OneUni.DTOs.User;

namespace OneUni.Interfaces.Services;

public interface IAuthService
{
    Task<Result<AuthResponseDTO>> RegisterAsync(SignUpRequestDTO request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponseDTO>> LoginAsync(LoginRequestDTO request, CancellationToken cancellationToken = default);
    Task<Result<AuthResponseDTO>> RefreshTokenAsync(RefreshTokenRequestDTO request, CancellationToken cancellationToken = default);
    Task<Result<UserDTO>> GetCurrentUserAsync(CancellationToken cancellationToken = default);
    Task<Result> LogoutAsync(CancellationToken cancellationToken = default);
    Task<Result> RevokeTokenAsync(RevokeTokenRequestDTO request, CancellationToken cancellationToken = default);
    Task<Result<bool>> ChangePasswordAsync(ChangePasswordRequestDTO request, CancellationToken cancellationToken = default);
    Task<Result> RevokeAllRefreshTokensAsync(CancellationToken cancellationToken = default);
    Task<Result<bool>> VerifyEmailAsync(string token, CancellationToken cancellationToken = default);
    Task<Result<bool>> RequestPasswordResetAsync(string email, CancellationToken cancellationToken = default);
    Task<Result> ResetPasswordAsync(string token, string newPassword, CancellationToken cancellationToken = default);
}

