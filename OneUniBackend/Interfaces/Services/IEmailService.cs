namespace OneUni.Interfaces.Services;

public interface IEmailService
{
    Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default);
    Task SendVerificationEmailAsync(string email, string token, CancellationToken cancellationToken = default);
    Task SendPasswordResetEmailAsync(string email, string token, CancellationToken cancellationToken = default);
}

