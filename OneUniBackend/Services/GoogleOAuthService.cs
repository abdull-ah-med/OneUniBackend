using System;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;
using OneUniBackend.DTOs.Auth;
using OneUniBackend.Entities;
using OneUniBackend.Interfaces.Repositories;
using OneUniBackend.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace OneUniBackend.Services;

public class GoogleOAuthService : IGoogleOAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _clientSecret;
    private readonly string _clientId;
    private readonly string _redirectURI;
    private readonly GoogleAuthorizationCodeFlow _flow;
    private readonly ILogger<GoogleOAuthService> _logger;
    public GoogleOAuthService(IConfiguration config, IUnitOfWork unitOfWork, ILogger<GoogleOAuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _clientId = config["GoogleAuth:ClientId"] ?? throw new ArgumentNullException("GoogleAuth:ClientId");
        _clientSecret = config["GoogleAuth:ClientSecret"] ?? throw new ArgumentNullException("GoogleAuth:ClientSecret");
        _redirectURI = config["GoogleAuth:RedirectUri"] ?? throw new ArgumentNullException("GoogleAuth:RedirectUri");
        _flow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
        {
            ClientSecrets = new ClientSecrets
            {
                ClientId = _clientId,
                ClientSecret = _clientSecret
            },
            Scopes = new[] { "openid", "email", "profile" }
        });
        _logger.LogInformation("GoogleOAuthService initialized with RedirectUri: {RedirectUri}", _redirectURI);
    }

    public async Task<User?> GetUserByGoogleIDAsync(string googleID, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _unitOfWork.UserExternalLoginRepository
                .FindUserByProviderAndKeyAsync("google", googleID, cancellationToken);
            return user;
        }
        catch (Exception ex)
        {
            throw new Exception("Error retrieving user by Google ID", ex);
        }
    }
    // Usecase of function: User is signing up for the first time using GoogleID
    public async Task<bool> SaveNewGoogleUserAsync(Guid userID, string googleuniqueID, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var userExternalLogin = new UserLogin
            {
                Providerkey = googleuniqueID,
                Loginprovider = "Google",
                Providerdisplayname = "Google",
                UserId = userID
            };
            await _unitOfWork.UserExternalLoginRepository.AddAsync(userExternalLogin);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return true;

        }
        catch (Exception)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return false;
        }
    }
    public async Task<GoogleUserInfo?> ExchangeCodeforUserInfoAsync(string code, CancellationToken cancellationToken = default)
    {
        try
        {
            TokenResponse token = await _flow.ExchangeCodeForTokenAsync(userId : "user", code, redirectUri: _redirectURI, taskCancellationToken: cancellationToken);
            var validPayload = await GoogleJsonWebSignature.ValidateAsync(token.IdToken, new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new[] {_clientId}
            });
            
            if (validPayload != null)
            {
                _logger.LogInformation("Successfully exchanged code for user info. Google User ID: {GoogleUserId}", validPayload.Subject);
                return new GoogleUserInfo
                {
                    GoogleUserId = validPayload.Subject,
                    UserEmail = validPayload.Email,
                    UserName = validPayload.Name,
                    isEmailVerified = validPayload.EmailVerified
                };
            }
        }
        catch (Exception ex)
        {
            throw new Exception("Error exchanging code for user info", ex);
        }
        return null;
    }
}
