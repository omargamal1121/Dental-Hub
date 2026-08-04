using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Auth;

namespace DentalHub.Application.Services.Auth
{
    public interface IRefreshTokenService
    {
       
        Task<Result<string>> GenerateAndStoreAsync(Guid userId, string securityStamp);

    
        
        Task<Result<RefreshTokenResponse>> RotateAsync(string rawToken);

        
        Task<Result<bool>> RevokeAsync(string rawToken);

        
        Task<Result<bool>> RevokeAllForUserAsync(Guid userId);
    }
}
