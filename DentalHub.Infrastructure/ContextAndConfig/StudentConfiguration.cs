using DentalHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalHub.Infrastructure.ContextAndConfig
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.User)
                   .WithOne(x => x.Student)
                   .HasForeignKey<Student>(x => x.Id)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(s => s.University)
                   .WithMany(u => u.Students)
                   .HasForeignKey(s => s.UniversityId)
                   .OnDelete(DeleteBehavior.Restrict);


			builder.HasQueryFilter(cr => !cr.User.IsDeleted);



		}
	}

}
