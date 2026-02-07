using DentalHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalHub.Infrastructure.ContextAndConfig
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
           
            builder.HasKey(x => x.UserId);

            
            builder.HasOne(x => x.User)
                   .WithOne(x => x.Patient)
                   .HasForeignKey<Patient>(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.PatientCases)
                   .WithOne(x => x.Patient)
                   .HasForeignKey(x => x.PatientId);
			builder.HasQueryFilter(cr => cr.DeleteAt == null);

		}
	}
}
