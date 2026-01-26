using DentalHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalHub.Infrastructure.ContextAndConfig
{
    public class CaseRequestConfiguration : IEntityTypeConfiguration<CaseRequest>
    {
        public void Configure(EntityTypeBuilder<CaseRequest> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.PatientCase)
                   .WithMany(x => x.CaseRequests)
                   .HasForeignKey(x => x.CaseId);

            builder.HasOne(x => x.Student)
                   .WithMany()
                   .HasForeignKey(x => x.StudentId);
        }
    }
}
