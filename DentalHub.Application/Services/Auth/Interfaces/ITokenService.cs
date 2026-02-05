using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Auth;
using DentalHub.Domain.Entities;

namespace DentalHub.Application.Services.Auth
{
    public interface ITokenService
    {
        Task<Result<string>> GenerateTokenAsync(User user);
        Task<Result<string>> GenerateTokenAsync(string userId);
    }
}
