using DentalHub.Domain.Entities;
using DentalHub.Domain.Utils;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DentalHub.Infrastructure.ContextAndConfig
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ContextApp>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ContextApp>>();

            try
            {
           
                // ── 1. Roles ──────────────────────────────────────────────────────────
                string[] roles = ["Doctor", "Student", "Patient", "Admin"];
                foreach (var role in roles)
                {
                    if (!await roleManager.RoleExistsAsync(role))
                        await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
                // ── 2. Universities ─────────────────────────────
                if (!await context.Universities.AnyAsync())
                {
                    var cairoId = Guid.Parse("11111111-1111-1111-1111-111111111111");
                    var ainShamsId = Guid.Parse("22222222-2222-2222-2222-222222222222");
                    var mansouraId = Guid.Parse("33333333-3333-3333-3333-333333333333");
                    var alexId = Guid.Parse("44444444-4444-4444-4444-444444444444");
                    var assiutId = Guid.Parse("55555555-5555-5555-5555-555555555555");
                    var benhaId = Guid.Parse("66666666-6666-6666-6666-666666666666");

                    var universities = new List<University>
                    {
                       new()
                       {
                          Id = cairoId,
                          Name = "Cairo University",
                          Email = "info@cu.edu.eg"
                       },
                       new()
                       {
                          Id = ainShamsId,
                          Name = "Ain Shams University",
                          Email = "info@asu.edu.eg"
                       },
                       new()
                       {
                          Id = mansouraId,
                          Name = "Mansoura University",
                          Email = "info@mu.edu.eg"
                       },
                       new()
                       {
                          Id = alexId,
                          Name = "Alexandria University",
                          Email = "info@alexu.edu.eg"
                       },
                       new()
                       {
                          Id = assiutId,
                          Name = "Assiut University",
                          Email = "info@aun.edu.eg"
                       },
                       new()
                       {
                          Id = benhaId,
                          Name = "Benha University",
                          Email = "info@bu.edu.eg"
                       }
                    };

                    await context.Universities.AddRangeAsync(universities);
                    await context.SaveChangesAsync();
                }
         
                if (!await context.UniversityMembers.AnyAsync())
                {
                    var universityMembers = new List<UniversityMember>
                    {
                        new()
                        {
                            Id         = 1,
                            UniversityId = Guid.Parse("66666666-6666-6666-6666-666666666666"), // Benha University
                            FullName   = "Dr. Ahmed Hassan",
                            Faculty    = "Faculty of Dentistry",
                            Department = "Oral Surgery",
                            Role       = "Doctor"
                        },
                        new()
                        {
                            Id         = 2,
                            UniversityId = Guid.Parse("55555555-5555-5555-5555-555555555555"), // Assiut University
                            FullName   = "Dr. Sara Mohamed",
                            Faculty    = "Faculty of Dentistry",
                            Department = "Orthodontics",
                            Role       = "Doctor"
                        },
                        new()
                        {
                            Id         = 3,
                            UniversityId = Guid.Parse("11111111-1111-1111-1111-111111111111"), // Cairo University
                            FullName   = "Omar Gamal",
                            Faculty    = "Faculty of Dentistry",
                            Department = "General Dentistry",
                            Role       = "Student"
                        },
                        new()
                        {
                            Id         = 4,
                            UniversityId = Guid.Parse("11111111-1111-1111-1111-111111111111"), // Cairo University
                            FullName   = "Nour Ali",
                            Faculty    = "Faculty of Dentistry",
                            Department = "General Dentistry",
                            Role       = "Student"
                        },
                        new()
                        {
                            Id         = 5,
                            UniversityId = Guid.Parse("22222222-2222-2222-2222-222222222222"), // Ain Shams University
                            FullName   = "Youssef Ibrahim",
                            Faculty    = "Faculty of Dentistry",
                            Department = "Pediatric Dentistry",
                            Role       = "Student"
                        }
                    };

                    await context.UniversityMembers.AddRangeAsync(universityMembers);
                    await context.SaveChangesAsync();
                    logger.LogInformation("✅ Seeded {Count} UniversityMembers.", universityMembers.Count);
                }

               
                if (!await context.Doctors.IgnoreQueryFilters().AnyAsync())
                {
                    // -- Doctor 1 --
                    var doc1Id    = Guid.Parse("01960000-0000-7000-8000-000000000001");

                    var doc1User = new User
                    {
                        Id             = doc1Id,
                        
                        FullName       = "Dr. Ahmed Hassan",
                        UserName       = "ahmed.hassan",
                        Email          = "ahmed.hassan@dentalhub.com",
                        EmailConfirmed = true,
                        PhoneNumber    = "01011111111"
                    };

                    var doc1 = new Doctor(doc1Id)
                    {
                        Name         = "Dr. Ahmed Hassan",
                        Specialty    = "Oral Surgery",
                        UniversityId = Guid.Parse("66666666-6666-6666-6666-666666666666") // ← Benha University  
                    };

                    // -- Doctor 2 --
                    var doc2Id    = Guid.Parse("01960000-0000-7000-8000-000000000002");

                    var doc2User = new User
                    {
                        Id             = doc2Id,
                       
                        FullName       = "Dr. Sara Mohamed",
                        UserName       = "sara.mohamed",
                        Email          = "sara.mohamed@dentalhub.com",
                        EmailConfirmed = true,
                        PhoneNumber    = "01022222222"
                    };

                    var doc2 = new Doctor(doc2Id)
                    {
                        Name         = "Dr. Sara Mohamed",
                        Specialty    = "Orthodontics",
                        UniversityId = Guid.Parse("55555555-5555-5555-5555-555555555555") // ← Assiut University
                    };

                    // Create users via UserManager so password hash is applied correctly
                    await CreateUserWithRoleAsync(userManager, doc1User, "Doctor", "Doctor@123", logger);
                    await CreateUserWithRoleAsync(userManager, doc2User, "Doctor", "Doctor@123", logger);

                    await context.Doctors.AddRangeAsync(doc1, doc2);
                    await context.SaveChangesAsync();
                    logger.LogInformation("✅ Seeded 2 Doctors.");
                }

                // ── 4. Students ───────────────────────────────────────────────────────
                // Each student's UniversityId must match an existing UniversityMember.UniversityId.
                if (!await context.Students.IgnoreQueryFilters().AnyAsync())
                {
                    // -- Student 1 --
                    var stu1Id    = Guid.Parse("01960000-0000-7000-8000-000000000003");

                    var stu1User = new User
                    {
                        Id             = stu1Id,
                        FullName       = "Omar Gamal",
                        UserName       = "omar.gamal",
                        Email          = "omar.gamal@dentalhub.com",
                        EmailConfirmed = true,
                        PhoneNumber    = "01033333333"
                    };

                    var stu1 = new Student(stu1Id)
                    {
                        Level        = 4,
                        UniversityId = Guid.Parse("11111111-1111-1111-1111-111111111111") // ← Cairo University
                    };

                    // -- Student 2 --
                    var stu2Id    = Guid.Parse("01960000-0000-7000-8000-000000000004");

                    var stu2User = new User
                    {
                        Id             = stu2Id,
                        FullName       = "Nour Ali",
                        UserName       = "nour.ali",
                        Email          = "nour.ali@dentalhub.com",
                        EmailConfirmed = true,
                        PhoneNumber    = "01044444444"
                    };

                    var stu2 = new Student(stu2Id)
                    {
                        Level        = 3,
                        UniversityId = Guid.Parse("11111111-1111-1111-1111-111111111111") // ← Cairo University
                    };

                    // -- Student 3 --
                    var stu3Id    = Guid.Parse("01960000-0000-7000-8000-000000000005");

                    var stu3User = new User
                    {
                        Id             = stu3Id,
                        FullName       = "Youssef Ibrahim",
                        UserName       = "youssef.ibrahim",
                        Email          = "youssef.ibrahim@dentalhub.com",
                        EmailConfirmed = true,
                        PhoneNumber    = "01055555555"
                    };

                    var stu3 = new Student(stu3Id)
                    {
                        Level        = 5,
                        UniversityId = Guid.Parse("22222222-2222-2222-2222-222222222222") // ← Ain Shams University
                    };

                    await CreateUserWithRoleAsync(userManager, stu1User, "Student", "Student@123", logger);
                    await CreateUserWithRoleAsync(userManager, stu2User, "Student", "Student@123", logger);
                    await CreateUserWithRoleAsync(userManager, stu3User, "Student", "Student@123", logger);

                    await context.Students.AddRangeAsync(stu1, stu2, stu3);
                    await context.SaveChangesAsync();
                    logger.LogInformation("✅ Seeded 3 Students.");
                }

                // ── 5. Patients ───────────────────────────────────────────────────────
                if (!await context.Patients.IgnoreQueryFilters().AnyAsync())
                {
                    // -- Patient 1 --
                    var pat1Id    = Guid.Parse("01960000-0000-7000-8000-000000000006");

                    var pat1User = new User
                    {
                        Id             = pat1Id,
                        FullName       = "Mona Tarek",
                        UserName       = "mona.tarek",
                        Email          = "mona.tarek@gmail.com",
                        EmailConfirmed = true,
                        PhoneNumber    = "01066666666"
                    };

                    var pat1 = new Patient(pat1Id)
                    {
                        Age   = 30,
                        Phone = "01066666666"
                    };

                    // -- Patient 2 --
                    var pat2Id    = Guid.Parse("01960000-0000-7000-8000-000000000007");

                    var pat2User = new User
                    {
                        Id             = pat2Id,
                        FullName       = "Karim Salah",
                        UserName       = "karim.salah",
                        Email          = "karim.salah@gmail.com",
                        EmailConfirmed = true,
                        PhoneNumber    = "01077777777"
                    };

                    var pat2 = new Patient(pat2Id)
                    {
                        Age   = 45,
                        Phone = "01077777777"
                    };

                    // -- Patient 3 --
                    var pat3Id    = Guid.Parse("01960000-0000-7000-8000-000000000008");

                    var pat3User = new User
                    {
                        Id             = pat3Id,
                        FullName       = "Layla Khaled",
                        UserName       = "layla.khaled",
                        Email          = "layla.khaled@gmail.com",
                        EmailConfirmed = true,
                        PhoneNumber    = "01088888888"
                    };

                    var pat3 = new Patient(pat3Id)
                    {
                        Age   = 25,
                        Phone = "01088888888"
                    };

                    await CreateUserWithRoleAsync(userManager, pat1User, "Patient", "Patient@123", logger);
                    await CreateUserWithRoleAsync(userManager, pat2User, "Patient", "Patient@123", logger);
                    await CreateUserWithRoleAsync(userManager, pat3User, "Patient", "Patient@123", logger);

                    await context.Patients.AddRangeAsync(pat1, pat2, pat3);
                    await context.SaveChangesAsync();
                    logger.LogInformation("✅ Seeded 3 Patients.");
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "❌ An error occurred while seeding the database.");
                throw;
            }
        }

        // ── Helper ────────────────────────────────────────────────────────────────────
        private static async Task CreateUserWithRoleAsync(
            UserManager<User> userManager,
            User user,
            string role,
            string password,
            ILogger logger)
        {
            // Skip if a user with the same email already exists
            if (await userManager.FindByEmailAsync(user.Email!) is not null)
                return;

            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role);
                logger.LogInformation("✅ Created user {Email} with role {Role}.", user.Email, role);
            }
            else
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                logger.LogWarning("⚠️ Failed to create user {Email}: {Errors}", user.Email, errors);
            }
        }
    }
}
