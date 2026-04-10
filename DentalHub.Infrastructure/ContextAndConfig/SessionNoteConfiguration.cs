using DentalHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalHub.Infrastructure.ContextAndConfig
{
    public class SessionNoteConfiguration : IEntityTypeConfiguration<SessionNote>
    {
        public void Configure(EntityTypeBuilder<SessionNote> builder)
        {
            builder.HasKey(x => x.Id);

          
			builder.HasQueryFilter(cr => cr.DeleteAt == null);

		}
	}
}
