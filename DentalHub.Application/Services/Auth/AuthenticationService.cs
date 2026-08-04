using Microsoft.EntityFrameworkCore;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Auth;
using DentalHub.Domain.Entities;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using DentalHub.Infrastructure.UnitOfWork;
using DentalHub.Application.Specification.Comman;

namespace DentalHub.Application.Services.Auth
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly ILogger<AuthenticationService> _logger;
        private readonly UserManager<User> _userManager;
        private readonly IRefreshTokenService _refreshTokenService;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IUnitOfWork _unitOfWork;

        public AuthenticationService(
            IUnitOfWork unitOfWork,
            ILogger<AuthenticationService> logger,
            UserManager<User> userManager,
            IRefreshTokenService refreshTokenService,
            ITokenService tokenService,
            IConfiguration configuration,
            IBackgroundJobClient backgroundJobClient)
        {
            _logger               = logger;
            _userManager          = userManager;
            _refreshTokenService  = refreshTokenService;
            _tokenService         = tokenService;
            _configuration        = configuration;
            _unitOfWork           = unitOfWork;
            _backgroundJobClient  = backgroundJobClient;
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Login
        // ─────────────────────────────────────────────────────────────────────────

        public async Task<Result<TokensDto>> LoginAsync(string emailOrPhone, string password)
        {
            try
            {
                User? user;
                if (emailOrPhone.Contains('@'))
                    user = await _userManager.FindByEmailAsync(emailOrPhone);
                else
                    user = await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == emailOrPhone);

                if (user == null)
                {
                    _logger.LogWarning("Login failed — user not found for {Identifier}", emailOrPhone);
                    return Result<TokensDto>.Failure("Invalid email or password.");
                }

                await EnsureLockoutEnabled(user);

                if (await _userManager.IsLockedOutAsync(user))
                    return Result<TokensDto>.Failure("Your account is currently locked. Please try again later.");

                if (!await _userManager.CheckPasswordAsync(user, password))
                    return await HandleFailedLoginAttemptAsync(user);

                await _userManager.ResetAccessFailedCountAsync(user);

                // Generate JWT access token
                var tokenResult = await _tokenService.GenerateTokenAsync(user);
                if (!tokenResult.IsSuccess || tokenResult.Data == null)
                {
                    _logger.LogError("Login — failed to generate access token: {Message}", tokenResult.Message);
                    return Result<TokensDto>.Failure("An error occurred during login.");
                }

                // Generate & store refresh token
                var refreshResult = await _refreshTokenService.GenerateAndStoreAsync(
                    user.Id, user.SecurityStamp ?? string.Empty);

                if (!refreshResult.IsSuccess || string.IsNullOrEmpty(refreshResult.Data))
                    _logger.LogError("Login — failed to generate refresh token for UserId: {UserId}", user.Id);

                var roles         = (await _userManager.GetRolesAsync(user)).ToList();
                var universityId  = await GetUserUniversityId(user, roles);

                return Result<TokensDto>.Success(
                    new TokensDto
                    {
                        Token        = tokenResult.Data,
                        RefreshToken = refreshResult.Data ?? string.Empty,
                        Roles        = roles,
                        PublicId     = user.Id,
                        universityId = universityId
                    },
                    "Login successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in LoginAsync.");
                return Result<TokensDto>.Failure("An error occurred during login.");
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Refresh Token
        // ─────────────────────────────────────────────────────────────────────────

        public async Task<Result<TokensDto>> RefreshTokenAsync(string refreshToken)
        {
            _logger.LogInformation("Executing {Method}", nameof(RefreshTokenAsync));

            var rotationResult = await _refreshTokenService.RotateAsync(refreshToken);
            if (!rotationResult.IsSuccess || rotationResult.Data == null)
            {
                _logger.LogWarning("RefreshTokenAsync — rotation failed: {Message}", rotationResult.Message);
                return Result<TokensDto>.Failure(
                    rotationResult.Errors?.FirstOrDefault() ?? "Invalid refresh token",
                    rotationResult.Status == 401 ? 401 : 400);
            }

            return Result<TokensDto>.Success(
                new TokensDto
                {
                    Token        = rotationResult.Data.Token,
                    RefreshToken = rotationResult.Data.RefreshToken
                },
                "Token refreshed");
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Logout
        // ─────────────────────────────────────────────────────────────────────────

        public async Task<Result<bool>> LogoutAsync(Guid userId, string refreshToken)
        {
            _logger.LogInformation("Executing {Method} for UserId: {UserId}", nameof(LogoutAsync), userId);

            // Revoke the specific refresh token
            if (!string.IsNullOrWhiteSpace(refreshToken))
            {
                var revokeResult = await _refreshTokenService.RevokeAsync(refreshToken);
                if (!revokeResult.IsSuccess)
                    _logger.LogWarning("LogoutAsync — failed to revoke refresh token for UserId: {UserId}", userId);
            }

            // Rotate SecurityStamp so the current JWT is invalidated on the next validation cycle
            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                _logger.LogError("LogoutAsync — user not found: {UserId}", userId);
                return Result<bool>.Failure("Invalid user ID");
            }

            var stampResult = await _userManager.UpdateSecurityStampAsync(user);
            if (!stampResult.Succeeded)
            {
                var errors = string.Join(", ", stampResult.Errors.Select(e => e.Description));
                _logger.LogError("LogoutAsync — failed to update SecurityStamp for UserId: {UserId}: {Errors}",
                    userId, errors);
            }

            return Result<bool>.Success(true, "Logout successful");
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Logout From All Devices
        // ─────────────────────────────────────────────────────────────────────────

        public async Task<Result<bool>> LogoutFromAllDevicesAsync(Guid userId)
        {
            _logger.LogInformation("Executing {Method} for UserId: {UserId}", nameof(LogoutFromAllDevicesAsync), userId);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                _logger.LogError("LogoutFromAllDevicesAsync — user not found: {UserId}", userId);
                return Result<bool>.Failure("Invalid user ID");
            }

            // 1. Rotate SecurityStamp — this invalidates ALL existing JWTs for the user
            var stampResult = await _userManager.UpdateSecurityStampAsync(user);
            if (!stampResult.Succeeded)
            {
                var errors = string.Join(", ", stampResult.Errors.Select(e => e.Description));
                _logger.LogError("LogoutFromAllDevicesAsync — SecurityStamp update failed for UserId: {UserId}: {Errors}",
                    userId, errors);
                return Result<bool>.Failure("Failed to invalidate sessions");
            }

            // 2. Hard-revoke all active refresh tokens in the DB
            var revokeResult = await _refreshTokenService.RevokeAllForUserAsync(userId);
            if (!revokeResult.IsSuccess)
                _logger.LogWarning("LogoutFromAllDevicesAsync — could not revoke all tokens for UserId: {UserId}", userId);

            _logger.LogInformation("LogoutFromAllDevicesAsync — all sessions invalidated for UserId: {UserId}", userId);
            return Result<bool>.Success(true, "Logged out from all devices");
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Private Helpers
        // ─────────────────────────────────────────────────────────────────────────

        private async Task<Guid?> GetUserUniversityId(User user, IList<string> roles)
        {
            if (roles.Contains("Admin"))
                return await GetUniversityIdAsync<Admin>(user.Id);

            if (roles.Contains("Doctor") || roles.Contains("ClinicalDoctor"))
            {
                var doctorId = await GetUniversityIdAsync<Doctor>(user.Id);
                if (doctorId != null && doctorId != Guid.Empty) return doctorId;
            }

            if (roles.Contains("Student") || roles.Contains("ClinicalDoctor"))
            {
                var studentId = await GetUniversityIdAsync<Student>(user.Id);
                if (studentId != null && studentId != Guid.Empty) return studentId;
            }

            return null;
        }

        private async Task<Guid?> GetUniversityIdAsync<T>(Guid userId) where T : class
        {
            return await _unitOfWork.GetRepository<T>()
                .GetByIdAsync(new BaseSpecificationWithProjection<T, Guid>(
                    x => EF.Property<Guid>(x, "Id") == userId,
                    x => EF.Property<Guid>(x, "UniversityId")
                ));
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
            var failedCount              = await _userManager.GetAccessFailedCountAsync(user);
            var maxFailedAttempts        = _configuration.GetValue("Security:LockoutPolicy:MaxFailedAttempts", 5);
            var lockoutDurationMinutes   = _configuration.GetValue("Security:LockoutPolicy:LockoutDurationMinutes", 15);
            var permanentLockoutAfter    = _configuration.GetValue("Security:LockoutPolicy:PermanentLockoutAfterAttempts", 10);

            if (failedCount >= permanentLockoutAfter)
            {
                user.LockoutEnd = DateTime.UtcNow.AddYears(100);
                await _userManager.UpdateAsync(user);

                if(user.Email is not null)
                _backgroundJobClient.Enqueue<IAccountEmailService>(
                    e => e.SendAccountLockedEmailAsync(user.Email, user.UserName??"user",
                        $"Multiple failed login attempts ({permanentLockoutAfter}+ times)"));
                return Result<TokensDto>.Failure(
                    "Your account has been permanently locked due to multiple failed login attempts. Please reset your password.");
            }

            if (failedCount >= maxFailedAttempts)
            {
                user.LockoutEnd = DateTime.UtcNow.AddMinutes(lockoutDurationMinutes);
                await _userManager.UpdateAsync(user);
                return Result<TokensDto>.Failure(
                    $"Too many failed login attempts. Please try again after {lockoutDurationMinutes} minutes.");
            }

            return Result<TokensDto>.Failure("Invalid email or password.");
        }
    }
}
