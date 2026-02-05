using DentalHub.Application.Common;
using DentalHub.Domain.Entities;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DentalHub.Application.Services.Auth
{
    public class PasswordService : IPasswordService
    {
        private readonly ILogger<PasswordService> _logger;
        private readonly UserManager<User> _userManager;
     //   private readonly IRefreshTokenService _refreshTokenService;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IAccountEmailService _accountEmailService;

        public PasswordService(
            IAccountEmailService accountEmailService,
            IBackgroundJobClient backgroundJobClient,
            ILogger<PasswordService> logger,
            UserManager<User> userManager)
        //    IRefreshTokenService refreshTokenService)
        {
            _accountEmailService = accountEmailService;
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
            _userManager = userManager;
        //    _refreshTokenService = refreshTokenService;
        }

        public async Task<Result<bool>> ChangePasswordAsync(string userid, string oldPassword, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userid);
                if (user == null)
                {
                    _logger.LogWarning("Change password failed: User {UserId} not found.", userid);
                    return Result<bool>.Failure("User not found.");
                }

                if (oldPassword.Equals(newPassword))
                {
                    _logger.LogWarning("Change password failed: User {UserId} tried same old password.", userid);
                    return Result<bool>.Failure("Can't use the same password.");
                }

                var result = await _userManager.ChangePasswordAsync(user, oldPassword, newPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogError("Change password failed for {UserId}. Errors: {Errors}", userid, errors);
                    return Result<bool>.Failure($"Errors: {errors}");
                }

            //    _backgroundJobClient.Enqueue(() => _refreshTokenService.RemoveRefreshTokenAsync(userid));
                _backgroundJobClient.Enqueue(() => _accountEmailService.SendEmailAfterChangePassAsync(user.UserName, user.Email));

                _logger.LogInformation("Password changed successfully for user {UserId}", userid);
                return Result<bool>.Success(true, "Password changed successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in ChangePasswordAsync for user {UserId}", userid);
                return Result<bool>.Failure("An unexpected error occurred.");
            }
        }

        public async Task<Result<bool>> RequestPasswordResetAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var encodedToken = System.Net.WebUtility.UrlEncode(token);

                    _backgroundJobClient.Enqueue(() =>
                        _accountEmailService.SendPasswordResetEmailAsync(user.Email, user.UserName, encodedToken));
                }

                // Always return success (don't reveal user existence)
                return Result<bool>.Success(true, "If the email exists, a reset link has been sent.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RequestPasswordResetAsync for email {Email}", email);
                return Result<bool>.Failure("An error occurred while requesting password reset.");
            }
        }

        public async Task<Result<bool>> ResetPasswordAsync(string email, string token, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    _logger.LogWarning("Password reset requested for non-existent user {Email}", email);
                    // Same behavior: don't reveal user existence
                    return Result<bool>.Success(true, "If your account exists, you will receive a confirmation email.");
                }

                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("Password reset failed for {Email}. Errors: {Errors}", email, errors);
                    return Result<bool>.Failure($"Reset Failed: {errors}");
                }

                _backgroundJobClient.Enqueue(() => _accountEmailService.SendPasswordResetSuccessEmailAsync(email));
          //      _backgroundJobClient.Enqueue(() => _refreshTokenService.RemoveRefreshTokenAsync(user.Id.ToString()));

                _logger.LogInformation("Password reset successful for user {Email}", email);
                return Result<bool>.Success(true, "Password has been reset successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ResetPasswordAsync for {Email}", email);
                return Result<bool>.Failure("An error occurred while resetting password.");
            }
        }
    }
}
