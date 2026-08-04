using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Auth;
using DentalHub.Application.Specification.Comman;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace DentalHub.Application.Services.Auth
{
  
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ILogger<RefreshTokenService> _logger;
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly TimeSpan _refreshTokenExpiry;

        public RefreshTokenService(
            ILogger<RefreshTokenService> logger,
            IConfiguration config,
            UserManager<User> userManager,
            ITokenService tokenService,
            IUnitOfWork unitOfWork)
        {
            _logger       = logger;
            _config       = config;
            _userManager  = userManager;
            _tokenService = tokenService;
            _unitOfWork   = unitOfWork;

            int expiryDays = _config.GetValue<int>("Jwt:RefreshTokenExpiryDays", 7);
            _refreshTokenExpiry = TimeSpan.FromDays(expiryDays);
        }

       
        public async Task<Result<string>> GenerateAndStoreAsync(Guid userId, string securityStamp)
        {
            _logger.LogInformation("GenerateAndStoreAsync — UserId: {UserId}", userId);

            try
            {
               
                var rawBytes = new byte[64];
                RandomNumberGenerator.Fill(rawBytes);
                string rawToken = Convert.ToBase64String(rawBytes)
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .Replace("=", "");

               
                string hash = HashToken(rawToken);

               
                var entity = new RefreshToken
                {
                    UserId        = userId,
                    TokenHash     = hash,
                    SecurityStamp = securityStamp,
                    CreatedAt     = DateTime.UtcNow,
                    ExpiresAt     = DateTime.UtcNow.Add(_refreshTokenExpiry),
                    IsRevoked     = false
                };

                await _unitOfWork.RefreshTokens.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Refresh token stored for UserId: {UserId}", userId);
                return Result<string>.Success(rawToken, "Refresh token generated");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating refresh token for UserId: {UserId}", userId);
                return Result<string>.Failure("Could not generate refresh token");
            }
        }

        /// <inheritdoc/>
        public async Task<Result<RefreshTokenResponse>> RotateAsync(string rawToken)
        {
            _logger.LogInformation("RotateAsync — rotating refresh token");

            try
            {
                // 1. Look up record by hash
                string hash   = HashToken(rawToken);
                var record = await FindByHashAsync(hash);

                if (record == null)
                {
                    _logger.LogWarning("RotateAsync — token not found");
                    return Result<RefreshTokenResponse>.Failure("Invalid refresh token", 401);
                }

                // 2. Validate: revoked?
                if (record.IsRevoked)
                {
                    _logger.LogWarning("RotateAsync — token already revoked. UserId: {UserId}", record.UserId);
                    // Possible token-reuse attack — revoke all tokens for this user as a safety measure
                    await RevokeAllForUserAsync(record.UserId);
                    return Result<RefreshTokenResponse>.Failure("Refresh token has been revoked", 401);
                }

                // 3. Validate: expired?
                if (DateTime.UtcNow >= record.ExpiresAt)
                {
                    _logger.LogWarning("RotateAsync — token expired. UserId: {UserId}", record.UserId);
                    return Result<RefreshTokenResponse>.Failure("Refresh token has expired", 401);
                }

                // 4. Validate: SecurityStamp matches current user stamp?
                var user = await _userManager.FindByIdAsync(record.UserId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("RotateAsync — user not found. UserId: {UserId}", record.UserId);
                    return Result<RefreshTokenResponse>.Failure("User not found", 401);
                }

                if (user.SecurityStamp != record.SecurityStamp)
                {
                    _logger.LogWarning("RotateAsync — SecurityStamp mismatch. UserId: {UserId}", record.UserId);
                    return Result<RefreshTokenResponse>.Failure("Session is no longer valid. Please login again.", 401);
                }

                // ── All validations passed ─────────────────────────────────────

                // 5. Revoke the current token (rotation)
                record.IsRevoked  = true;
                record.RevokedAt  = DateTime.UtcNow;
                _unitOfWork.RefreshTokens.Update(record);

                // 6. Generate new JWT access token
                var accessTokenResult = await _tokenService.GenerateTokenAsync(user);
                if (!accessTokenResult.IsSuccess || string.IsNullOrEmpty(accessTokenResult.Data))
                {
                    _logger.LogError("RotateAsync — failed to generate access token. UserId: {UserId}", user.Id);
                    return Result<RefreshTokenResponse>.Failure("Could not generate access token");
                }

                // 7. Generate & store new refresh token
                var newRefreshResult = await GenerateAndStoreAsync(user.Id, user.SecurityStamp ?? string.Empty);
                if (!newRefreshResult.IsSuccess || string.IsNullOrEmpty(newRefreshResult.Data))
                {
                    _logger.LogError("RotateAsync — failed to generate new refresh token. UserId: {UserId}", user.Id);
                    return Result<RefreshTokenResponse>.Failure("Could not generate new refresh token");
                }

                // Flush the revocation of the old record (GenerateAndStoreAsync already saved the new one)
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("RotateAsync — rotation successful. UserId: {UserId}", user.Id);

                return Result<RefreshTokenResponse>.Success(new RefreshTokenResponse
                {
                    Token        = accessTokenResult.Data,
                    RefreshToken = newRefreshResult.Data
                }, "Token refreshed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during refresh token rotation");
                return Result<RefreshTokenResponse>.Failure("An error occurred while refreshing the token");
            }
        }

        /// <inheritdoc/>
        public async Task<Result<bool>> RevokeAsync(string rawToken)
        {
            _logger.LogInformation("RevokeAsync — revoking single refresh token");

            try
            {
                string hash   = HashToken(rawToken);
                var record = await FindByHashAsync(hash);

                if (record == null)
                {
                    _logger.LogWarning("RevokeAsync — token not found (may already be removed)");
                    return Result<bool>.Success(true, "Token not found or already revoked");
                }

                if (record.IsRevoked)
                {
                    _logger.LogInformation("RevokeAsync — token already revoked");
                    return Result<bool>.Success(true, "Token already revoked");
                }

                record.IsRevoked  = true;
                record.RevokedAt  = DateTime.UtcNow;
                _unitOfWork.RefreshTokens.Update(record);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("RevokeAsync — token revoked. UserId: {UserId}", record.UserId);
                return Result<bool>.Success(true, "Token revoked");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking refresh token");
                return Result<bool>.Failure("Failed to revoke token");
            }
        }

        /// <inheritdoc/>
        public async Task<Result<bool>> RevokeAllForUserAsync(Guid userId)
        {
            _logger.LogInformation("RevokeAllForUserAsync — UserId: {UserId}", userId);

            try
            {
                var activeTokens = await _unitOfWork.RefreshTokens.GetAllAsync(
                    new BaseSpecification<RefreshToken>(
                        rt => rt.UserId == userId && !rt.IsRevoked));

                if (activeTokens.Count == 0)
                {
                    _logger.LogInformation("RevokeAllForUserAsync — no active tokens for UserId: {UserId}", userId);
                    return Result<bool>.Success(true, "No active tokens to revoke");
                }

                var now = DateTime.UtcNow;
                foreach (var token in activeTokens)
                {
                    token.IsRevoked  = true;
                    token.RevokedAt  = now;
                    _unitOfWork.RefreshTokens.Update(token);
                }

                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("RevokeAllForUserAsync — {Count} tokens revoked for UserId: {UserId}",
                    activeTokens.Count, userId);

                return Result<bool>.Success(true, $"{activeTokens.Count} token(s) revoked");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking all refresh tokens for UserId: {UserId}", userId);
                return Result<bool>.Failure("Failed to revoke all tokens");
            }
        }

        // ─────────────────────────────────────────────────────────────────────────
        // Private helpers
        // ─────────────────────────────────────────────────────────────────────────

        /// <summary>Returns the lowercase SHA-256 hex digest of the raw token.</summary>
        private static string HashToken(string rawToken)
        {
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(rawToken));
            return Convert.ToHexString(hashBytes).ToLowerInvariant();
        }

        /// <summary>Finds a RefreshToken entity by its hash, or returns null.</summary>
        private async Task<RefreshToken?> FindByHashAsync(string hash)
        {
            return await _unitOfWork.RefreshTokens.GetByIdAsync(
                new BaseSpecification<RefreshToken>(rt => rt.TokenHash == hash));
        }
    }
}