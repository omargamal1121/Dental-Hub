using DentalHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalHub.Infrastructure.ContextAndConfig
{
    public class MediaConfiguration : IEntityTypeConfiguration<Media>
    {
        public void Configure(EntityTypeBuilder<Media> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Session)
                   .WithMany(x => x.Medias)
                   .HasForeignKey(x => x.SessionId);
			builder.HasQueryFilter(cr => cr.DeleteAt == null);

		}
	}
}
