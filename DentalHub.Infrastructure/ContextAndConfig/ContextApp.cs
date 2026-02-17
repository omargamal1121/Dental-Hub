using DentalHub.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DentalHub.Infrastructure.ContextAndConfig
{
    public class ContextApp : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ContextApp(DbContextOptions<ContextApp> options) : base(options)
        {
        }

      
        public DbSet<Student> Students { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<PatientCase> PatientCases { get; set; }
        public DbSet<CaseRequest> CaseRequests { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Media> Medias { get; set; }
        public DbSet<SessionNote> SessionNotes { get; set; }
        public DbSet<UniversityMember>  UniversityMembers { get; set; }
        public DbSet<CaseType>   CaseTypes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ContextApp).Assembly);

            // Configure PublicId for all entities that have it
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                var publicIdProperty = entity.FindProperty("PublicId");
                if (publicIdProperty != null && publicIdProperty.ClrType == typeof(string))
                {
                    modelBuilder.Entity(entity.ClrType)
                        .Property("PublicId")
                        .IsRequired()
                        .HasMaxLength(30); // Base62 for 128-bit is ~22 chars

                    modelBuilder.Entity(entity.ClrType)
                        .HasIndex("PublicId")
                        .IsUnique();
                }
            }
        }
    }
}
