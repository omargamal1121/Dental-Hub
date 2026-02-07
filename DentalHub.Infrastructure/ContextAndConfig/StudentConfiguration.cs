using DentalHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalHub.Infrastructure.ContextAndConfig
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(x => x.UserId);

            builder.HasOne(x => x.User)
                   .WithOne(x => x.Student)
                   .HasForeignKey<Student>(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);
			builder.HasQueryFilter(cr => cr.DeleteAt == null);


		}
	}
}
