using DentalHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalHub.Infrastructure.ContextAndConfig
{
    public class DoctorConfiguration : IEntityTypeConfiguration<Doctor>
    {
        public void Configure(EntityTypeBuilder<Doctor> builder)
        {
            builder.HasKey(x => x.UserId);

			builder.HasOne(d => d.User)
	.WithOne(u => u.Doctor)
	.HasForeignKey<Doctor>(d => d.UserId)
				   .OnDelete(DeleteBehavior.Cascade);
			builder.HasQueryFilter(cr => cr.DeleteAt == null);

		}
	}
}
