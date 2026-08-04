namespace DentalHub.Domain.Entities
{
    public class RefreshToken
    {
        public RefreshToken()
        {
            Id = Guid.CreateVersion7();
        }

        
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

      
        public string TokenHash { get; set; } = string.Empty;

       
        public string SecurityStamp { get; set; } = string.Empty;

      
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }

        public bool IsRevoked { get; set; } = false;

    
        public DateTime? RevokedAt { get; set; }

     
        public User User { get; set; } = null!;

        public bool IsActive => !IsRevoked && DateTime.UtcNow < ExpiresAt;

		
	}
}
