using System;
using OneUniBackend.Entities;
using OneUniBackend.Interfaces.Repositories;
using OneUniBackend.Interfaces.Services;

namespace OneUniBackend.Services;

public class GoogleOAuthService : IGoogleOAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    public GoogleOAuthService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<User?> GetUserByGoogleIDAsync(string googleID, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var user = await _unitOfWork.UserExternalLoginRepository
                .FindUserByProviderAndKeyAsync("Google", googleID, cancellationToken);
            return user;
        }
        catch (Exception ex)
        {
            throw new Exception("Error retrieving user by Google ID", ex);
        }
    }
    // Usecase of function: User is signing up for the first time using GoogleID
    public async Task<bool> SaveNewGoogleUserAsync(Guid userID, string googleID, string googleSecret, CancellationToken cancellationToken = default){
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var userExternalLogin = new UserLogin
            {
                Providerkey = googleID,
                Loginprovider = "Google",
                Providerdisplayname = "Google",
                UserId = userID           
            };
            await _unitOfWork.UserExternalLoginRepository.AddAsync(userExternalLogin);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
            return true;

        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            return false;
        }
    }
}
