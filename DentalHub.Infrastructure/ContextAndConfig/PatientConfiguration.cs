using DentalHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalHub.Infrastructure.ContextAndConfig
{
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            // Primary key
            builder.HasKey(x => x.UserId);

            // One-to-One: Patient -> User
            builder.HasOne(x => x.User)
                   .WithOne(x => x.Patient)
                   .HasForeignKey<Patient>(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Patient -> PatientCases
            builder.HasMany(x => x.PatientCases)
                   .WithOne(x => x.Patient)
                   .HasForeignKey(x => x.PatientId);
        }
    }
}
