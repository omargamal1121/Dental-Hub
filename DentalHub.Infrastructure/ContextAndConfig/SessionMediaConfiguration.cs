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

            builder.Property(x => x.MediaUrl)
                   .IsRequired();

            builder.Property(x => x.CloudinaryPublicId)
                   .IsRequired();

            builder.HasOne(x => x.Session)
                   .WithMany(x => x.Medias)
                   .HasForeignKey(x => x.SessionId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.SessionNote)
                   .WithMany(x => x.Medias)
                   .HasForeignKey(x => x.SessionNoteId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.Patient)
                   .WithMany()
                   .HasForeignKey(x => x.PatientId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.PatientCase)
                   .WithMany()
                   .HasForeignKey(x => x.PatientCaseId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.CaseType)
                   .WithMany()
                   .HasForeignKey(x => x.CaseTypeId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasQueryFilter(cr => cr.DeleteAt == null);
        }
    }
}
