using DentalHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace DentalHub.Infrastructure.ContextAndConfig
{
    public class CaseRequestConfiguration : IEntityTypeConfiguration<CaseRequest>
    {
        public void Configure(EntityTypeBuilder<CaseRequest> builder)
        {
			
			builder.HasKey(x => x.Id);

			builder
				.HasOne(c => c.Student)
				.WithMany(s => s.CaseRequests)
				.HasForeignKey(c => c.StudentId);

			builder
				.HasOne(c => c.Doctor)
				.WithMany(d => d.CaseRequests)
				.HasForeignKey(c => c.DoctorId);

			builder
				.HasOne(c => c.PatientCase)
				.WithMany(p => p.CaseRequests)
				.HasForeignKey(c => c.PatientCaseId);
			builder.HasQueryFilter(cr => cr.DeleteAt == null);

		}
	}
}
