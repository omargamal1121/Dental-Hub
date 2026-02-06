using DentalHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalHub.Infrastructure.ContextAndConfig
{
    public class PatientCaseConfiguration : IEntityTypeConfiguration<PatientCase>
    {
        public void Configure(EntityTypeBuilder<PatientCase> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Patient)
                   .WithMany(x => x.PatientCases)
                   .HasForeignKey(x => x.PatientId);

            builder.HasMany(x => x.CaseRequests)
                   .WithOne(x => x.PatientCase)
                   .HasForeignKey(x => x.PatientCaseId);

            builder.HasMany(x => x.Sessions)
                   .WithOne(x => x.PatientCase)
                   .HasForeignKey(x => x.CaseId);
			builder.HasQueryFilter(cr => cr.DeleteAt == null);

		}
	}
}
