using DentalHub.Domain.Entities;
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

                // ── 2. Universities ───────────────────────────────────────────────────
                var cairoId = Guid.Parse("11111111-1111-1111-1111-111111111111");
                var ainShamsId = Guid.Parse("22222222-2222-2222-2222-222222222222");
                var mansouraId = Guid.Parse("33333333-3333-3333-3333-333333333333");
                var alexId = Guid.Parse("44444444-4444-4444-4444-444444444444");
                var assiutId = Guid.Parse("55555555-5555-5555-5555-555555555555");
                var benhaId = Guid.Parse("66666666-6666-6666-6666-666666666666");

                if (!await context.Universities.AnyAsync())
                {
                    var universities = new List<University>
                    {
                        new() { Id = cairoId,    Name = "Cairo University",      Email = "info@cu.edu.eg"    },
                        new() { Id = ainShamsId, Name = "Ain Shams University",  Email = "info@asu.edu.eg"   },
                        new() { Id = mansouraId, Name = "Mansoura University",   Email = "info@mu.edu.eg"    },
                        new() { Id = alexId,     Name = "Alexandria University", Email = "info@alexu.edu.eg" },
                        new() { Id = assiutId,   Name = "Assiut University",     Email = "info@aun.edu.eg"   },
                        new() { Id = benhaId,    Name = "Benha University",      Email = "info@bu.edu.eg"    },
                    };

                    await context.Universities.AddRangeAsync(universities);
                    await context.SaveChangesAsync();
                    logger.LogInformation("✅ Seeded 6 Universities.");
                }

                // ── 3. UniversityMembers ──────────────────────────────────────────────
                // Registry: one Doctor + one Student per university = 12 entries total.
                // Use the UniversityId from here when creating a Doctor or Student account.
                //
                // | University            | UniversityId                         |
                // |-----------------------|--------------------------------------|
                // | Cairo University      | 11111111-1111-1111-1111-111111111111 |
                // | Ain Shams University  | 22222222-2222-2222-2222-222222222222 |
                // | Mansoura University   | 33333333-3333-3333-3333-333333333333 |
                // | Alexandria University | 44444444-4444-4444-4444-444444444444 |
                // | Assiut University     | 55555555-5555-5555-5555-555555555555 |
                // | Benha University      | 66666666-6666-6666-6666-666666666666 |
                if (await context.UniversityMembers.CountAsync() < 12)
                {
                    // Remove any incomplete/old entries and re-seed correctly
                    context.UniversityMembers.RemoveRange(context.UniversityMembers);
                    await context.SaveChangesAsync();

                    var universityMembers = new List<UniversityMember>
                    {
                        // Cairo University      → 11111111-1111-1111-1111-111111111111
                        new() { Id = 1,  UniversityId = cairoId,    FullName = "Dr. Ahmed Hassan",   Faculty = "Faculty of Dentistry", Department = "General Dentistry",   Role = "Doctor"  },
                        new() { Id = 2,  UniversityId = cairoId,    FullName = "Omar Gamal",          Faculty = "Faculty of Dentistry", Department = "General Dentistry",   Role = "Student" },

                        // Ain Shams University  → 22222222-2222-2222-2222-222222222222
                        new() { Id = 3,  UniversityId = ainShamsId, FullName = "Dr. Sara Mohamed",   Faculty = "Faculty of Dentistry", Department = "Pediatric Dentistry", Role = "Doctor"  },
                        new() { Id = 4,  UniversityId = ainShamsId, FullName = "Nour Ali",            Faculty = "Faculty of Dentistry", Department = "Pediatric Dentistry", Role = "Student" },

                        // Mansoura University   → 33333333-3333-3333-3333-333333333333
                        new() { Id = 5,  UniversityId = mansouraId, FullName = "Dr. Hossam Naguib",  Faculty = "Faculty of Dentistry", Department = "Oral Surgery",        Role = "Doctor"  },
                        new() { Id = 6,  UniversityId = mansouraId, FullName = "Youssef Ibrahim",    Faculty = "Faculty of Dentistry", Department = "Oral Surgery",        Role = "Student" },

                        // Alexandria University → 44444444-4444-4444-4444-444444444444
                        new() { Id = 7,  UniversityId = alexId,     FullName = "Dr. Dina Farouk",    Faculty = "Faculty of Dentistry", Department = "Orthodontics",        Role = "Doctor"  },
                        new() { Id = 8,  UniversityId = alexId,     FullName = "Farida Samir",       Faculty = "Faculty of Dentistry", Department = "Orthodontics",        Role = "Student" },

                        // Assiut University     → 55555555-5555-5555-5555-555555555555
                        new() { Id = 9,  UniversityId = assiutId,   FullName = "Dr. Tarek Mostafa",  Faculty = "Faculty of Dentistry", Department = "Orthodontics",        Role = "Doctor"  },
                        new() { Id = 10, UniversityId = assiutId,   FullName = "Mahmoud Essam",      Faculty = "Faculty of Dentistry", Department = "Orthodontics",        Role = "Student" },

                        // Benha University      → 66666666-6666-6666-6666-666666666666
                        new() { Id = 11, UniversityId = benhaId,    FullName = "Dr. Rania Adel",     Faculty = "Faculty of Dentistry", Department = "Oral Surgery",        Role = "Doctor"  },
                        new() { Id = 12, UniversityId = benhaId,    FullName = "Salma Wael",         Faculty = "Faculty of Dentistry", Department = "Oral Surgery",        Role = "Student" },
                    };

                    await context.UniversityMembers.AddRangeAsync(universityMembers);
                    await context.SaveChangesAsync();
                    logger.LogInformation("✅ Seeded 12 UniversityMembers (1 Doctor + 1 Student per university).");
                }

                // ── 4. Doctors ────────────────────────────────────────────────────────
                // One Doctor per university. UniversityId matches the registry above.
                if (!await context.Doctors.IgnoreQueryFilters().AnyAsync())
                {
                    // Cairo - ID 1
                    var doc1Id = Guid.Parse("01960000-0000-7000-8000-000000000001");
                    var doc1User = new User { Id = doc1Id, FullName = "Dr. Ahmed Hassan", UserName = "ahmed.hassan", Email = "ahmed.hassan@dentalhub.com", EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000001" };
                    var doc1 = new Doctor(doc1Id) { Name = "Dr. Ahmed Hassan", Specialty = "General Dentistry", UniversityId = cairoId };

                    // Ain Shams - ID 3
                    var doc2Id = Guid.Parse("01960000-0000-7000-8000-000000000002");
                    var doc2User = new User { Id = doc2Id, FullName = "Dr. Sara Mohamed", UserName = "sara.mohamed", Email = "sara.mohamed@dentalhub.com", EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000003" };
                    var doc2 = new Doctor(doc2Id) { Name = "Dr. Sara Mohamed", Specialty = "Pediatric Dentistry", UniversityId = ainShamsId };

                    // Mansoura - ID 5
                    var doc3Id = Guid.Parse("01960000-0000-7000-8000-000000000009");
                    var doc3User = new User { Id = doc3Id, FullName = "Dr. Hossam Naguib", UserName = "hossam.naguib", Email = "hossam.naguib@dentalhub.com", EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000005" };
                    var doc3 = new Doctor(doc3Id) { Name = "Dr. Hossam Naguib", Specialty = "Oral Surgery", UniversityId = mansouraId };

                    // Alexandria - ID 7
                    var doc4Id = Guid.Parse("01960000-0000-7000-8000-000000000010");
                    var doc4User = new User { Id = doc4Id, FullName = "Dr. Dina Farouk", UserName = "dina.farouk", Email = "dina.farouk@dentalhub.com", EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000007" };
                    var doc4 = new Doctor(doc4Id) { Name = "Dr. Dina Farouk", Specialty = "Orthodontics", UniversityId = alexId };

                    // Assiut - ID 9
                    var doc5Id = Guid.Parse("01960000-0000-7000-8000-000000000011");
                    var doc5User = new User { Id = doc5Id, FullName = "Dr. Tarek Mostafa", UserName = "tarek.mostafa", Email = "tarek.mostafa@dentalhub.com", EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000009" };
                    var doc5 = new Doctor(doc5Id) { Name = "Dr. Tarek Mostafa", Specialty = "Orthodontics", UniversityId = assiutId };

                    // Benha - ID 11
                    var doc6Id = Guid.Parse("01960000-0000-7000-8000-000000000012");
                    var doc6User = new User { Id = doc6Id, FullName = "Dr. Rania Adel", UserName = "rania.adel", Email = "rania.adel@dentalhub.com", EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000011" };
                    var doc6 = new Doctor(doc6Id) { Name = "Dr. Rania Adel", Specialty = "Oral Surgery", UniversityId = benhaId };

                    await CreateUserWithRoleAsync(userManager, doc1User, "Doctor", "Doctor@123", logger);
                    await CreateUserWithRoleAsync(userManager, doc2User, "Doctor", "Doctor@123", logger);
                    await CreateUserWithRoleAsync(userManager, doc3User, "Doctor", "Doctor@123", logger);
                    await CreateUserWithRoleAsync(userManager, doc4User, "Doctor", "Doctor@123", logger);
                    await CreateUserWithRoleAsync(userManager, doc5User, "Doctor", "Doctor@123", logger);
                    await CreateUserWithRoleAsync(userManager, doc6User, "Doctor", "Doctor@123", logger);

                    await context.Doctors.AddRangeAsync(doc1, doc2, doc3, doc4, doc5, doc6);
                    await context.SaveChangesAsync();
                    logger.LogInformation("✅ Seeded 6 Doctors (one per university) with unique numbers.");
                }

                // ── 5. Students ───────────────────────────────────────────────────────
                // One Student per university. UniversityId matches the registry above.
                if (!await context.Students.IgnoreQueryFilters().AnyAsync())
                {
                    // Cairo - ID 2
                    var stu1Id = Guid.Parse("01960000-0000-7000-8000-000000000003");
                    var stu1User = new User { Id = stu1Id, FullName = "Omar Gamal", UserName = "omar.gamal", Email = "omar.gamal@dentalhub.com", EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000002" };
                    var stu1 = new Student(stu1Id) { Level = 4, UniversityId = cairoId };

                    // Ain Shams - ID 4
                    var stu2Id = Guid.Parse("01960000-0000-7000-8000-000000000004");
                    var stu2User = new User { Id = stu2Id, FullName = "Nour Ali", UserName = "nour.ali", Email = "nour.ali@dentalhub.com", EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000004" };
                    var stu2 = new Student(stu2Id) { Level = 3, UniversityId = ainShamsId };

                    // Mansoura - ID 6
                    var stu3Id = Guid.Parse("01960000-0000-7000-8000-000000000005");
                    var stu3User = new User { Id = stu3Id, FullName = "Youssef Ibrahim", UserName = "youssef.ibrahim", Email = "youssef.ibrahim@dentalhub.com", EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000006" };
                    var stu3 = new Student(stu3Id) { Level = 5, UniversityId = mansouraId };

                    // Alexandria - ID 8
                    var stu4Id = Guid.Parse("01960000-0000-7000-8000-000000000013");
                    var stu4User = new User { Id = stu4Id, FullName = "Farida Samir", UserName = "farida.samir", Email = "farida.samir@dentalhub.com", EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000008" };
                    var stu4 = new Student(stu4Id) { Level = 2, UniversityId = alexId };

                    // Assiut - ID 10
                    var stu5Id = Guid.Parse("01960000-0000-7000-8000-000000000014");
                    var stu5User = new User { Id = stu5Id, FullName = "Mahmoud Essam", UserName = "mahmoud.essam", Email = "mahmoud.essam@dentalhub.com", EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000010" };
                    var stu5 = new Student(stu5Id) { Level = 1, UniversityId = assiutId };

                    // Benha - ID 12
                    var stu6Id = Guid.Parse("01960000-0000-7000-8000-000000000015");
                    var stu6User = new User { Id = stu6Id, FullName = "Salma Wael", UserName = "salma.wael", Email = "salma.wael@dentalhub.com", EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000012" };
                    var stu6 = new Student(stu6Id) { Level = 4, UniversityId = benhaId };

                    await CreateUserWithRoleAsync(userManager, stu1User, "Student", "Student@123", logger);
                    await CreateUserWithRoleAsync(userManager, stu2User, "Student", "Student@123", logger);
                    await CreateUserWithRoleAsync(userManager, stu3User, "Student", "Student@123", logger);
                    await CreateUserWithRoleAsync(userManager, stu4User, "Student", "Student@123", logger);
                    await CreateUserWithRoleAsync(userManager, stu5User, "Student", "Student@123", logger);
                    await CreateUserWithRoleAsync(userManager, stu6User, "Student", "Student@123", logger);

                    await context.Students.AddRangeAsync(stu1, stu2, stu3, stu4, stu5, stu6);
                    await context.SaveChangesAsync();
                    logger.LogInformation("✅ Seeded 6 Students (one per university) with unique numbers.");
                }

                // ── 6. Patients ───────────────────────────────────────────────────────
                if (!await context.Patients.IgnoreQueryFilters().AnyAsync())
                {
                    // ID 13
                    var pat1Id = Guid.Parse("01960000-0000-7000-8000-000000000006");
                    var pat1User = new User { Id = pat1Id, FullName = "Mona Tarek", UserName = "mona.tarek", Email = "mona.tarek@gmail.com", EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000013" };
                    var pat1 = new Patient(pat1Id) { Age = 30, Phone = "01000000013" };

                    // ID 14
                    var pat2Id = Guid.Parse("01960000-0000-7000-8000-000000000007");
                    var pat2User = new User { Id = pat2Id, FullName = "Karim Salah", UserName = "karim.salah", Email = "karim.salah@gmail.com", EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000014" };
                    var pat2 = new Patient(pat2Id) { Age = 45, Phone = "01000000014" };

                    // ID 15
                    var pat3Id = Guid.Parse("01960000-0000-7000-8000-000000000008");
                    var pat3User = new User { Id = pat3Id, FullName = "Layla Khaled", UserName = "layla.khaled", Email = "layla.khaled@gmail.com", EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000015" };
                    var pat3 = new Patient(pat3Id) { Age = 25, Phone = "01000000015" };

                    await CreateUserWithRoleAsync(userManager, pat1User, "Patient", "Patient@123", logger);
                    await CreateUserWithRoleAsync(userManager, pat2User, "Patient", "Patient@123", logger);
                    await CreateUserWithRoleAsync(userManager, pat3User, "Patient", "Patient@123", logger);

                    await context.Patients.AddRangeAsync(pat1, pat2, pat3);
                    await context.SaveChangesAsync();
                    logger.LogInformation("✅ Seeded 3 Patients with unique numbers.");
                }

                // ── 7. Admins ─────────────────────────────────────────────────────────
                if (!await context.Admins.IgnoreQueryFilters().AnyAsync())
                {
                    // ID 16
                    var adminId = Guid.Parse("01960000-0000-7000-8000-000000000016");
                    var adminUser = new User { Id = adminId, FullName = "System Admin", UserName = "admin", Email = "admin@dentalhub.com", EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000016" };
                    var admin = new Admin(adminId) { Phone = "01000000016", IsSuperAdmin = true };

                    await CreateUserWithRoleAsync(userManager, adminUser, "Admin", "Admin@123", logger);
                    await context.Admins.AddAsync(admin);
                    await context.SaveChangesAsync();
                    logger.LogInformation("✅ Seeded Admin Account with unique number.");
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
