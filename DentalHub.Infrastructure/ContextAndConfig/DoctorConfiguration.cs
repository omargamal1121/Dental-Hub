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

            builder.HasOne(x => x.User)
                   .WithOne(x => x.Doctor)
                   .HasForeignKey<Doctor>(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
