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

            builder.Property(x => x.Note)
                   .IsRequired()
                   .HasMaxLength(2000);

            builder.HasOne(x => x.Session)
                   .WithMany(x => x.SessionNotes)
                   .HasForeignKey(x => x.SessionId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasQueryFilter(cr => cr.DeleteAt == null);
        }
    }
}
