namespace DentalHub.Application.DTOs.Auth
{
    
    public class TokensDto
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public IList<string> Roles { get; set; } = new List<string>();
        public Guid PublicId { get; set; }
        public Guid? universityId { get; set; }
    }

    
    public record RefreshTokenRequest(string RefreshToken);

    
    public record LogoutRequest(string RefreshToken);

    
    public class RefreshTokenData
    {
        public string UserId { get; set; } = string.Empty;
        public string SecurityStamp { get; set; } = string.Empty;
    }

    
    public class RefreshTokenResponse
    {
        public string Token { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
