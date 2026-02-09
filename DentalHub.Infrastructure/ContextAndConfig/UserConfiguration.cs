using DentalHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalHub.Infrastructure.ContextAndConfig
{
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			builder.HasKey(x => x.Id);

			builder.HasOne(u => u.Admin)
				   .WithOne(a => a.User)
				   .HasForeignKey<Admin>(a => a.UserId)
				   .OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(u => u.Student)
				   .WithOne(s => s.User)
				   .HasForeignKey<Student>(s => s.UserId)
				   .OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(u => u.Doctor)
				   .WithOne(d => d.User)
				   .HasForeignKey<Doctor>(d => d.UserId)
				   .OnDelete(DeleteBehavior.Cascade);

			builder.HasMany(u => u.UserRoles)
				   .WithOne()
				   .HasForeignKey(ur => ur.UserId);
		}
	}

}
