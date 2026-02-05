namespace DentalHub.Application.Services.Auth
{
    public interface IAccountEmailService
    {
        Task SendValidationEmailAsync(string email, string userId, string token, string frontendUrl);
        Task SendPasswordResetEmailAsync(string email, string username, string token);
        Task SendPasswordResetSuccessEmailAsync(string email);
        Task SendAccountLockedEmailAsync(string email, string username, string reason = "Multiple failed login attempts");
        Task SendWelcomeEmailAsync(string email, string username);
        Task SendEmailAfterChangePassAsync(string username, string email);
    }
}
