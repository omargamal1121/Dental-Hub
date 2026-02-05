using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Auth;

namespace DentalHub.Application.Services.Auth
{
    public interface IAuthenticationService
    {
        Task<Result<TokensDto>> LoginAsync(string email, string password);
        Task<Result<bool>> LogoutAsync(string userId);
   //     Task<Result<TokensDto>> RefreshTokenAsync();
    //    Task RemoveRefreshTokenAsync(string refreshToken);
    }
}
