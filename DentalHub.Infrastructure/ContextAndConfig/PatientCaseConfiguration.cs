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
            builder.HasOne(x => x.CaseType)
                   .WithMany(x => x.PatientCases)
                   .HasForeignKey(x => x.CaseTypeId);
			builder.HasQueryFilter(cr => cr.DeleteAt == null);
           

		}
	}
    public class CaseTypeConfiguration : IEntityTypeConfiguration<CaseType>
    {
        public void Configure(EntityTypeBuilder<CaseType> builder)
        {
            builder.HasKey(ct => ct.Id);
            builder.HasIndex(ct => ct.Name).IsUnique();
            builder.HasMany(ct=>ct.Medias).WithOne(m=>m.CaseType).HasForeignKey(ct=>ct.CaseTypeId);
			builder.HasQueryFilter(cr => cr.DeleteAt == null);



		}
	}
}
