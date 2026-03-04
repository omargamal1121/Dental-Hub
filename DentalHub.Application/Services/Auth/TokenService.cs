using DentalHub.Application.Common;
using DentalHub.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace DentalHub.Application.Services.Auth
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _secretkey;
        private readonly double _expiresInMinutes;
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;

        public TokenService(ILogger<TokenService> logger, IConfiguration config, UserManager<User> userManager)
        {
            _logger = logger;
            _userManager = userManager;
            _config = config;

            _secretkey = _config["Jwt:Key"]
                ?? throw new ArgumentNullException(nameof(_secretkey), "Jwt:Key is missing in appsettings.json");
            _issuer = _config["Jwt:Issuer"]
                ?? throw new ArgumentNullException(nameof(_issuer), "Jwt:Issuer is missing in appsettings.json");
            _audience = _config["Jwt:Audience"]
                ?? throw new ArgumentNullException(nameof(_audience), "Jwt:Audience is missing in appsettings.json");

            if (_secretkey.Length < 32)
            {
                throw new InvalidOperationException("JWT secret key must be at least 32 characters long");
            }

            if (!double.TryParse(_config["Jwt:ExpiresInMinutes"], out double expiresInMinutes))
            {
                _logger.LogWarning("JWT ExpiresInMinutes is missing, using default (15 minutes).");
                expiresInMinutes = 15;
            }
            _expiresInMinutes = expiresInMinutes;
        }

        public async Task<Result<string>> GenerateTokenAsync(User user)
        {
            _logger.LogInformation("Generating Access Token for User ID: {UserId}", user.Id);
            return await GenerateTokenInternalAsync(user);
        }

        public async Task<Result<string>> GenerateTokenAsync(string userId)
        {
            _logger.LogInformation("Generating Access Token for User ID: {UserId}", userId);

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogWarning("User not found for ID: {UserId}", userId);
                return Result<string>.Failure("User not found");
            }

            return await GenerateTokenInternalAsync(user);
        }

        private async Task<Result<string>> GenerateTokenInternalAsync(User user)
        {
            _logger.LogInformation("Generating Access Token for User ID: {UserId}", user.Id);

            if (await _userManager.IsLockedOutAsync(user))
            {
                _logger.LogWarning("Cannot generate token for locked out user: {UserId}", user.Id);
                return Result<string>.Failure("User account is locked");
            }

            var jti = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32));

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Jti, jti),
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
            };

            var roles = await _userManager.GetRolesAsync(user);
            var userclaims = await _userManager.GetClaimsAsync(user);

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            foreach (var claim in userclaims)
            {
                claims.Add(claim);
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretkey));
            var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var notBefore = DateTime.UtcNow.AddSeconds(-30);
            var expires = DateTime.UtcNow.AddMinutes(_expiresInMinutes);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                notBefore: notBefore,
                expires: expires,
                claims: claims,
                signingCredentials: signingCredentials
            );

            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            _logger.LogInformation("Access Token generated successfully for User ID: {UserId}, expires at {ExpiresAt}",
                user.Id, expires);

            return Result<string>.Success(tokenString, "Access Token generated successfully");
        }
    }
}