using DentalHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class AdminConfiguration : IEntityTypeConfiguration<Admin>
{
    public void Configure(EntityTypeBuilder<Admin> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasOne(a => a.User)
               .WithOne(u => u.Admin)
               .HasForeignKey<Admin>(a => a.Id)
               .OnDelete(DeleteBehavior.Cascade);
		builder.HasQueryFilter(cr => !cr.User.IsDeleted);

	}
}
