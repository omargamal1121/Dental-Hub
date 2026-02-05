using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Auth;
using DentalHub.Domain.Entities;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Security.Cryptography;
using System.Text.Json;

namespace DentalHub.Application.Services.Auth
{
    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly ILogger<RefreshTokenService> _logger;
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenHelper;
        private readonly IConnectionMultiplexer _redis;
        private readonly IDatabase _database;
        private readonly IBackgroundJobClient _backgroundJobClient;

        private readonly TimeSpan _refreshTokenExpiry;

        public RefreshTokenService(
            IBackgroundJobClient backgroundJobClient,
            ITokenService tokenHelper,
            ILogger<RefreshTokenService> logger,
            IConnectionMultiplexer redis,
            IConfiguration config,
            UserManager<User> userManager)
        {
            _backgroundJobClient = backgroundJobClient;
            _tokenHelper = tokenHelper;
            _logger = logger;
            _userManager = userManager;
            _redis = redis;
            _database = _redis.GetDatabase();
            _config = config;

            int expiryHours = _config.GetValue<int>("JwtSettings:RefreshTokenExpiryHours", 168); // 7 days default
            _refreshTokenExpiry = TimeSpan.FromHours(expiryHours);
        }

        private static string GenerateRedisKey(string token) => $"RefreshToken:{token}";

        public async Task<Result<RefreshTokenResponse>> RefreshTokenAsync(string refreshToken)
        {
            _logger.LogInformation("RefreshTokenAsync started");

            string? value = await _database.StringGetAsync(GenerateRedisKey(refreshToken));

            if (string.IsNullOrEmpty(value))
            {
                _logger.LogWarning("Refresh token not found in Redis");
                return Result<RefreshTokenResponse>.Failure("Invalid or expired refresh token");
            }

            RefreshTokenData? data;
            try
            {
                data = JsonSerializer.Deserialize<RefreshTokenData>(value);
                if (data == null)
                {
                    _logger.LogError("Failed to deserialize refresh token data");
                    return Result<RefreshTokenResponse>.Failure("Invalid token data");
                }
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Error deserializing refresh token data");
                return Result<RefreshTokenResponse>.Failure("Invalid token format");
            }

            var userValidation = await ValidateRefreshTokenAsync(data.UserId, data.SecurityStamp);
            if (!userValidation.IsSuccess)
            {
                await RemoveRefreshTokenAsync(refreshToken); 
                return Result<RefreshTokenResponse>.Failure(userValidation.Message);
            }

            var newRefresh = await GenerateRefreshTokenAsync(data.UserId, data.SecurityStamp);
            if (!newRefresh.IsSuccess || string.IsNullOrEmpty(newRefresh.Data))
            {
                _logger.LogError("Failed to generate new refresh token");
                return Result<RefreshTokenResponse>.Failure("Unable to generate new refresh token");
            }

            await RemoveRefreshTokenAsync(refreshToken);

            var user = await _userManager.FindByIdAsync(data.UserId);
            if (user == null)
            {
                _logger.LogError("User not found after validation: {UserId}", data.UserId);
                return Result<RefreshTokenResponse>.Failure("User not found");
            }

            var tokenResult = await _tokenHelper.GenerateTokenAsync(user);
            if (!tokenResult.IsSuccess || string.IsNullOrEmpty(tokenResult.Data))
            {
                _logger.LogError("Failed to generate access token");
                return Result<RefreshTokenResponse>.Failure("Unable to generate access token");
            }

            return Result<RefreshTokenResponse>.Success(new RefreshTokenResponse
            {
                Token = tokenResult.Data,
                RefreshToken = newRefresh.Data
            }, "Token Refreshed");
        }

        public async Task<Result<string>> GenerateRefreshTokenAsync(string userId, string securityStamp)
        {
            _logger.LogInformation("Generating Refresh Token for User ID: {UserId}", userId);

            var tokenBytes = new byte[64];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(tokenBytes);
            }

            string token = Convert.ToBase64String(tokenBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");

            var tokenData = new RefreshTokenData
            {
                UserId = userId,
                SecurityStamp = securityStamp
            };

            var value = JsonSerializer.Serialize(tokenData);

            bool stored = await _database.StringSetAsync(
                GenerateRedisKey(token),
                value,
                _refreshTokenExpiry,
                When.Always
            );

            if (!stored)
            {
                _logger.LogError("Failed to store refresh token in Redis for User ID: {UserId}", userId);
                return Result<string>.Failure("Could not store refresh token");
            }

            _logger.LogInformation("RefreshToken generated and stored for User ID: {UserId}", userId);
            return Result<string>.Success(token, "RefreshToken Generated");
        }

        public async Task<Result<bool>> RemoveRefreshTokenAsync(string token)
        {
            try
            {
                string key = GenerateRedisKey(token);
                bool deleted = await _database.KeyDeleteAsync(key);

                if (!deleted)
                {
                    _logger.LogWarning("RefreshToken not found or already deleted: {Token}", token);
                    return Result<bool>.Success(true, "Token already removed or expired");
                }

                _logger.LogInformation("Successfully removed RefreshToken");
                return Result<bool>.Success(true, "Token Removed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing refresh token");
                return Result<bool>.Failure("Failed to remove RefreshToken");
            }
        }

        public async Task<Result<bool>> ValidateRefreshTokenAsync(string userId, string securityStamp)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("Validation failed: User not found {UserId}", userId);
                return Result<bool>.Failure("User not found");
            }

            if (user.SecurityStamp != securityStamp)
            {
                _logger.LogWarning("Validation failed: Security stamp mismatch for {UserId}", userId);
                return Result<bool>.Failure("Security stamp mismatch");
            }

            return Result<bool>.Success(true, "Valid");
        }
    }
}