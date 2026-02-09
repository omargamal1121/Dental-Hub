using DentalHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DentalHub.Infrastructure.ContextAndConfig
{
	public class UniversityMemberConfiguration : IEntityTypeConfiguration<UniversityMember>
    {
        public void Configure(EntityTypeBuilder<UniversityMember> builder)
        {
            builder.HasKey(x => x.Id);
            builder.HasIndex(um=>um.UniversityId).IsUnique();


		}
	}

}
