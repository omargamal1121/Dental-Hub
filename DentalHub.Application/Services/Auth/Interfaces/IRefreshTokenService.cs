using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Auth;

namespace DentalHub.Application.Services.Auth
{
    public interface IRefreshTokenService
    {
        Task<Result<RefreshTokenResponse>> RefreshTokenAsync(string refreshToken);
        Task<Result<string>> GenerateRefreshTokenAsync(string userId, string securityStamp);
        Task<Result<bool>> RemoveRefreshTokenAsync(string token);
        Task<Result<bool>> ValidateRefreshTokenAsync(string userId, string securityStamp);
    }
}