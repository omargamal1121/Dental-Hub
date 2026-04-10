using DentalHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalHub.Infrastructure.ContextAndConfig
{
    public class DiagnosisConfiguration : IEntityTypeConfiguration<Diagnosis>
    {
        public void Configure(EntityTypeBuilder<Diagnosis> builder)
        {
            builder.HasKey(d => d.Id);

            builder.HasOne(d => d.PatientCase)
                   .WithMany(pc => pc.Diagnosiss)
                   .HasForeignKey(d => d.PatientCaseId);

            builder.HasOne(d => d.CaseType)
                   .WithMany(ct=>ct.Diagnoses)
                   .HasForeignKey(d => d.CaseTypeId);

			builder.HasOne(d => d.User)
				.WithMany()
				.HasForeignKey(d => d.CreatedById)
				.IsRequired(false)  
				.OnDelete(DeleteBehavior.SetNull);
            builder.HasQueryFilter(d=>d.DeleteAt == null);
		}
    }
}
