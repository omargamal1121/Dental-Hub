namespace DentalHub.Application.DTOs.Auth
{
    public class TokensDto
    {
        public string Token { get; set; }
        public IList<string> Roles { get; set; }
    }

    public class RefreshTokenData
    {
        public string UserId { get; set; }
        public string SecurityStamp { get; set; }
    }

    public class RefreshTokenResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
