using DentalHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalHub.Infrastructure.ContextAndConfig
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.HasKey(x => x.Id);

			builder.HasOne(d => d.User)
			.WithOne(u => u.Doctor)
			.HasForeignKey<Doctor>(d => d.Id)
			.OnDelete(DeleteBehavior.Cascade);


            builder.HasOne(d => d.University)
            .WithMany(u => u.Doctors)
            .HasForeignKey(d => d.UniversityId)
            .OnDelete(DeleteBehavior.Restrict);


            builder.HasQueryFilter(cr => cr.DeleteAt == null);

		}
	}
}
