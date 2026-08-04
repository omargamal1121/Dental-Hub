using DentalHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalHub.Infrastructure.ContextAndConfig
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(x => x.Id);

            builder.Property(x => x.TokenHash)
                .IsRequired()
                .HasMaxLength(64);  

            builder.Property(x => x.SecurityStamp)
                .IsRequired()
                .HasMaxLength(36);

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.ExpiresAt)
                .IsRequired();

            builder.Property(x => x.IsRevoked)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(x => x.RevokedAt)
                .IsRequired(false);

        
            builder.HasIndex(x => x.TokenHash)
                .IsUnique()
                .HasDatabaseName("IX_RefreshTokens_TokenHash");

           
            builder.HasIndex(x => x.UserId)
                .HasDatabaseName("IX_RefreshTokens_UserId");

            
            builder.HasOne(rt => rt.User)
                .WithMany()
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
