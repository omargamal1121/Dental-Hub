using DentalHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalHub.Infrastructure.ContextAndConfig
{
    public class SessionMediaConfiguration : IEntityTypeConfiguration<SessionMedia>
    {
        public void Configure(EntityTypeBuilder<SessionMedia> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Session)
                   .WithMany(x => x.SessionMedias)
                   .HasForeignKey(x => x.SessionId);
        }
    }
}
