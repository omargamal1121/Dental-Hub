using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Auth;

namespace DentalHub.Application.Services.Auth
{
    public interface IAuthenticationService
    {
        Task<Result<TokensDto>> LoginAsync(string emailOrPhone, string password);
        Task<Result<TokensDto>> RefreshTokenAsync(string refreshToken);
        Task<Result<bool>> LogoutAsync(Guid userId, string refreshToken);
        Task<Result<bool>> LogoutFromAllDevicesAsync(Guid userId);
    }
}
