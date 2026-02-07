using DentalHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalHub.Infrastructure.ContextAndConfig
{
    public class SessionConfiguration : IEntityTypeConfiguration<Session>
    {
        public void Configure(EntityTypeBuilder<Session> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.PatientCase)
                   .WithMany(x => x.Sessions)
                   .HasForeignKey(x => x.CaseId);

            builder.HasMany(x => x.Medias)
                   .WithOne(x => x.Session)
                   .HasForeignKey(x => x.SessionId);

            builder.HasMany(x => x.SessionNotes)
                   .WithOne(x => x.Session)
                   .HasForeignKey(x => x.SessionId);
			builder.HasQueryFilter(cr => cr.DeleteAt == null);

		}
	}
}
