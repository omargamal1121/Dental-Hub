using DentalHub.Application.Common;

namespace DentalHub.Application.Services.Auth
{
    public interface IPasswordService
    {
        Task<Result<bool>> ChangePasswordAsync(string userid, string oldPassword, string newPassword);
        Task<Result<bool>> RequestPasswordResetAsync(string email);
        Task<Result<bool>> ResetPasswordAsync(string email, string token, string newPassword);
    }
}
