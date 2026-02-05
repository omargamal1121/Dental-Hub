using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Auth;
using DentalHub.Domain.Entities;
using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace DentalHub.Application.Services.Auth
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILogger<AuthenticationService> _logger;
        private readonly UserManager<User> _userManager;
      //  private readonly IRefreshTokenService _refreshTokenService;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBackgroundJobClient _backgroundJobClient;

        private const string RefreshCookieName = "Refresh";

        public AuthenticationService(
            IHttpContextAccessor httpContextAccessor,
            ILogger<AuthenticationService> logger,
            UserManager<User> userManager,
      //      IRefreshTokenService refreshTokenService,
            ITokenService tokenService,
            IConfiguration configuration,
            IBackgroundJobClient backgroundJobClient)
        {
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
            _userManager = userManager;
       //     _refreshTokenService = refreshTokenService;
            _tokenService = tokenService;
            _configuration = configuration;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<Result<TokensDto>> LoginAsync(string email, string password)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("Login Failed: Email not found for {Email}", email);
                    return Result<TokensDto>.Failure("Invalid email or password.");
                }

                await EnsureLockoutEnabled(user);

                if (await _userManager.IsLockedOutAsync(user))
                    return Result<TokensDto>.Failure("Your account is currently locked. Please try again later.");

                if (!await _userManager.CheckPasswordAsync(user, password))
                    return await HandleFailedLoginAttemptAsync(user);

                await _userManager.ResetAccessFailedCountAsync(user);

                var tokenResult = await _tokenService.GenerateTokenAsync(user);
                if (!tokenResult.IsSuccess || tokenResult.Data == null)
                {
                    _logger.LogError("Failed to generate token: {Message}", tokenResult.Message);
                    return Result<TokensDto>.Failure("An error occurred during login.");
                }

                //var refreshTokenResult = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id.ToString(), user.SecurityStamp ?? "");
                //if (refreshTokenResult.IsSuccess && refreshTokenResult.Data != null)
                //{
                //    SetRefreshCookie(refreshTokenResult.Data);
                //}
                //else
                //{
                //    _logger.LogError("Failed to generate refresh token: {Message}", refreshTokenResult.Message);
                //}

                var roles = (await _userManager.GetRolesAsync(user)).ToList();
                return Result<TokensDto>.Success(new TokensDto { Token = tokenResult.Data, Roles = roles }, "Login successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in LoginAsync.");
                return Result<TokensDto>.Failure("An error occurred during login.");
            }
        }

        public async Task<Result<bool>> LogoutAsync(string userId)
        {
            _logger.LogInformation("Executing {Method}", nameof(LogoutAsync));

            //var refreshToken = GetRefreshCookie();
            //if (refreshToken != null)
            //    _backgroundJobClient.Enqueue<IAuthenticationService>(s => s.RemoveRefreshTokenAsync(refreshToken));

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError("No user found with ID: {UserId}", userId);
                return Result<bool>.Failure("Invalid user ID");
            }

            var updateResult = await _userManager.UpdateSecurityStampAsync(user);
            if (!updateResult.Succeeded)
            {
                var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
                _logger.LogError("Failed to update security stamp for user {UserId}: {Errors}", userId, errors);
            }
            ExpireRefreshCookie();
            return Result<bool>.Success(true, "Logout Successful");
        }

        //public async Task<Result<TokensDto>> RefreshTokenAsync()
        //{
        //    var refreshToken = GetRefreshCookie();
        //    if (string.IsNullOrEmpty(refreshToken))
        //    {
        //        _logger.LogWarning("Refresh token not found in cookies.");
        //        return Result<TokensDto>.Failure("Please login again");
        //    }

        //    //var tokenResult = await _refreshTokenService.RefreshTokenAsync(refreshToken);
        //    //if (!tokenResult.IsSuccess || tokenResult.Data == null)
        //    //{
        //    //    _logger.LogWarning("Failed to refresh token. Removing refresh token.");
        //    //    await RemoveRefreshTokenAsync(refreshToken);
        //    //    ExpireRefreshCookie();
        //    //    return Result<TokensDto>.Failure("Failed to generate token. Please login again.");
        //    //}
        //    SetRefreshCookie(tokenResult.Data.RefreshToken);

        //    var token = new TokensDto
        //    {
        //        Token = tokenResult.Data.Token
        //    };
        //    return Result<TokensDto>.Success(token, "Token generated");
        //}

        //public async Task RemoveRefreshTokenAsync(string refreshToken)
        //{
        //    try
        //    {
        //        await _refreshTokenService.RemoveRefreshTokenAsync(refreshToken);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error in {Method}", nameof(RemoveRefreshTokenAsync));
        //    }
        //}

        #region Private Helpers

        private string? GetRefreshCookie() =>
            _httpContextAccessor?.HttpContext?.Request.Cookies[RefreshCookieName];

        private void SetRefreshCookie(string token)
        {
            _httpContextAccessor?.HttpContext?.Response.Cookies.Append(
                RefreshCookieName,
                token,
                new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTimeOffset.UtcNow.AddDays(7)
                });
        }

        private void ExpireRefreshCookie()
        {
            _httpContextAccessor?.HttpContext?.Response.Cookies.Append(
                RefreshCookieName,
                string.Empty,
                new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(-1),
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
        }

        private async Task EnsureLockoutEnabled(User user)
        {
            if (!user.LockoutEnabled)
            {
                user.LockoutEnabled = true;
                await _userManager.UpdateAsync(user);
            }
        }

        private async Task<Result<TokensDto>> HandleFailedLoginAttemptAsync(User user)
        {
            await _userManager.AccessFailedAsync(user);
            var failedCount = await _userManager.GetAccessFailedCountAsync(user);
            var maxFailedAttempts = _configuration.GetValue("Security:LockoutPolicy:MaxFailedAttempts", 5);
            var lockoutDurationMinutes = _configuration.GetValue("Security:LockoutPolicy:LockoutDurationMinutes", 15);
            var permanentLockoutAfterAttempts = _configuration.GetValue("Security:LockoutPolicy:PermanentLockoutAfterAttempts", 10);

            if (failedCount >= permanentLockoutAfterAttempts)
            {
                user.LockoutEnd = DateTime.UtcNow.AddYears(100);
                await _userManager.UpdateAsync(user);
                 _backgroundJobClient.Enqueue<IAccountEmailService>(e => e.SendAccountLockedEmailAsync(user.Email, user.UserName, $"Multiple Failed login attempts ({permanentLockoutAfterAttempts}+ times)"));
                return Result<TokensDto>.Failure("Your account has been permanently locked due to multiple failed login attempts. Please reset your password.");
            }

            if (failedCount >= maxFailedAttempts)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(lockoutDurationMinutes);
                await _userManager.UpdateAsync(user);
                // BackgroundJob.Enqueue<IAccountEmailService>(e => e.SendAccountLockedEmailAsync(user.Email, user.UserName, $"Multiple Failed login attempts ({maxFailedAttempts}+ times)"));
                return Result<TokensDto>.Failure($"Too many failed login attempts. Please try again after {lockoutDurationMinutes} minutes.");
            }

            return Result<TokensDto>.Failure("Invalid email or password.");
        }

        #endregion
    }
}
