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
				var tanta = Guid.Parse("77777777-7777-7777-7777-777777777777");
				var zagazig = Guid.Parse("88888888-8888-8888-8888-888888888888");
				var minia = Guid.Parse("99999999-9999-9999-9999-999999999999");
				var suez = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");

				if (!await context.Universities.AnyAsync())
				{
					var universities = new List<University>
					{
						new() { Id = cairoId,    Name = "Cairo University",      Email = "info@cu.edu.eg"       },
						new() { Id = ainShamsId, Name = "Ain Shams University",  Email = "info@asu.edu.eg"      },
						new() { Id = mansouraId, Name = "Mansoura University",   Email = "info@mu.edu.eg"       },
						new() { Id = alexId,     Name = "Alexandria University", Email = "info@alexu.edu.eg"    },
						new() { Id = assiutId,   Name = "Assiut University",     Email = "info@aun.edu.eg"      },
						new() { Id = benhaId,    Name = "Benha University",      Email = "info@bu.edu.eg"       },
						new() { Id = tanta,      Name = "Tanta University",      Email = "info@tanta.edu.eg"    },
						new() { Id = zagazig,    Name = "Zagazig University",    Email = "info@zu.edu.eg"       },
						new() { Id = minia,      Name = "Minia University",      Email = "info@minia.edu.eg"    },
						new() { Id = suez,       Name = "Suez Canal University", Email = "info@suez.edu.eg"     },
					};

					await context.Universities.AddRangeAsync(universities);
					await context.SaveChangesAsync();
					logger.LogInformation("✅ Seeded 10 Universities.");
				}

				// ── 3. UniversityMembers ──────────────────────────────────────────────
				if (await context.UniversityMembers.CountAsync() < 50)
				{
					context.UniversityMembers.RemoveRange(context.UniversityMembers);
					await context.SaveChangesAsync();

					var universityMembers = new List<UniversityMember>
					{
                        // Cairo University
                        new() { Id = 1,  UniversityId = cairoId, FullName = "Dr. Ahmed Hassan",      Faculty = "Faculty of Dentistry", Department = "General Dentistry",   Role = "Doctor"  },
						new() { Id = 2,  UniversityId = cairoId, FullName = "Omar Gamal",            Faculty = "Faculty of Dentistry", Department = "General Dentistry",   Role = "Student" },
						new() { Id = 3,  UniversityId = cairoId, FullName = "Dr. Mona Ibrahim",      Faculty = "Faculty of Dentistry", Department = "Endodontics",         Role = "Doctor"  },
						new() { Id = 4,  UniversityId = cairoId, FullName = "Zeyad Khaled",          Faculty = "Faculty of Dentistry", Department = "General Dentistry",   Role = "Student" },
						new() { Id = 5,  UniversityId = cairoId, FullName = "Dr. Hassan Youssef",    Faculty = "Faculty of Dentistry", Department = "Prosthodontics",      Role = "Doctor"  },
						new() { Id = 6,  UniversityId = cairoId, FullName = "Aya Mahmoud",           Faculty = "Faculty of Dentistry", Department = "General Dentistry",   Role = "Student" },
						new() { Id = 7,  UniversityId = cairoId, FullName = "Mariam Fathy",          Faculty = "Faculty of Dentistry", Department = "Orthodontics",        Role = "Student" },
						new() { Id = 8,  UniversityId = cairoId, FullName = "Ali Sherif",            Faculty = "Faculty of Dentistry", Department = "Oral Surgery",        Role = "Student" },

                        // Ain Shams University
                        new() { Id = 9,  UniversityId = ainShamsId, FullName = "Dr. Sara Mohamed",   Faculty = "Faculty of Dentistry", Department = "Pediatric Dentistry", Role = "Doctor"  },
						new() { Id = 10, UniversityId = ainShamsId, FullName = "Nour Ali",           Faculty = "Faculty of Dentistry", Department = "Pediatric Dentistry", Role = "Student" },
						new() { Id = 11, UniversityId = ainShamsId, FullName = "Dr. Khaled Fawzi",   Faculty = "Faculty of Dentistry", Department = "Periodontics",        Role = "Doctor"  },
						new() { Id = 12, UniversityId = ainShamsId, FullName = "Hana Ahmed",         Faculty = "Faculty of Dentistry", Department = "General Dentistry",   Role = "Student" },
						new() { Id = 13, UniversityId = ainShamsId, FullName = "Dr. Amira Saad",     Faculty = "Faculty of Dentistry", Department = "Restorative",         Role = "Doctor"  },
						new() { Id = 14, UniversityId = ainShamsId, FullName = "Karim Hassan",       Faculty = "Faculty of Dentistry", Department = "Orthodontics",        Role = "Student" },

                        // Mansoura University
                        new() { Id = 15, UniversityId = mansouraId, FullName = "Dr. Hossam Naguib",  Faculty = "Faculty of Dentistry", Department = "Oral Surgery",        Role = "Doctor"  },
						new() { Id = 16, UniversityId = mansouraId, FullName = "Youssef Ibrahim",    Faculty = "Faculty of Dentistry", Department = "Oral Surgery",        Role = "Student" },
						new() { Id = 17, UniversityId = mansouraId, FullName = "Dr. Yasmin Ali",     Faculty = "Faculty of Dentistry", Department = "Restorative",         Role = "Doctor"  },
						new() { Id = 18, UniversityId = mansouraId, FullName = "Laila Mostafa",      Faculty = "Faculty of Dentistry", Department = "General Dentistry",   Role = "Student" },
						new() { Id = 19, UniversityId = mansouraId, FullName = "Mohamed Samy",       Faculty = "Faculty of Dentistry", Department = "Endodontics",         Role = "Student" },

                        // Alexandria University
                        new() { Id = 20, UniversityId = alexId, FullName = "Dr. Dina Farouk",        Faculty = "Faculty of Dentistry", Department = "Orthodontics",        Role = "Doctor"  },
						new() { Id = 21, UniversityId = alexId, FullName = "Farida Samir",           Faculty = "Faculty of Dentistry", Department = "Orthodontics",        Role = "Student" },
						new() { Id = 22, UniversityId = alexId, FullName = "Dr. Waleed Hamdy",       Faculty = "Faculty of Dentistry", Department = "Periodontics",        Role = "Doctor"  },
						new() { Id = 23, UniversityId = alexId, FullName = "Salma Adel",             Faculty = "Faculty of Dentistry", Department = "General Dentistry",   Role = "Student" },
						new() { Id = 24, UniversityId = alexId, FullName = "Yara Nabil",             Faculty = "Faculty of Dentistry", Department = "Pediatric Dentistry", Role = "Student" },

                        // Assiut University
                        new() { Id = 25, UniversityId = assiutId, FullName = "Dr. Tarek Mostafa",    Faculty = "Faculty of Dentistry", Department = "Orthodontics",        Role = "Doctor"  },
						new() { Id = 26, UniversityId = assiutId, FullName = "Mahmoud Essam",        Faculty = "Faculty of Dentistry", Department = "Orthodontics",        Role = "Student" },
						new() { Id = 27, UniversityId = assiutId, FullName = "Dr. Noha Kamal",       Faculty = "Faculty of Dentistry", Department = "Endodontics",         Role = "Doctor"  },
						new() { Id = 28, UniversityId = assiutId, FullName = "Ibrahim Ashraf",       Faculty = "Faculty of Dentistry", Department = "General Dentistry",   Role = "Student" },

                        // Benha University
                        new() { Id = 29, UniversityId = benhaId, FullName = "Dr. Rania Adel",        Faculty = "Faculty of Dentistry", Department = "Oral Surgery",        Role = "Doctor"  },
						new() { Id = 30, UniversityId = benhaId, FullName = "Salma Wael",            Faculty = "Faculty of Dentistry", Department = "Oral Surgery",        Role = "Student" },
						new() { Id = 31, UniversityId = benhaId, FullName = "Dr. Hisham Fouad",      Faculty = "Faculty of Dentistry", Department = "Prosthodontics",      Role = "Doctor"  },
						new() { Id = 32, UniversityId = benhaId, FullName = "Nada Omar",             Faculty = "Faculty of Dentistry", Department = "General Dentistry",   Role = "Student" },

                        // Tanta University
                        new() { Id = 33, UniversityId = tanta, FullName = "Dr. Sherif Mohsen",       Faculty = "Faculty of Dentistry", Department = "Oral Surgery",        Role = "Doctor"  },
						new() { Id = 34, UniversityId = tanta, FullName = "Ahmed Saber",             Faculty = "Faculty of Dentistry", Department = "General Dentistry",   Role = "Student" },
						new() { Id = 35, UniversityId = tanta, FullName = "Dr. Eman Tawfik",         Faculty = "Faculty of Dentistry", Department = "Pediatric Dentistry", Role = "Doctor"  },
						new() { Id = 36, UniversityId = tanta, FullName = "Rana Magdy",              Faculty = "Faculty of Dentistry", Department = "Orthodontics",        Role = "Student" },

                        // Zagazig University
                        new() { Id = 37, UniversityId = zagazig, FullName = "Dr. Tamer Nabil",       Faculty = "Faculty of Dentistry", Department = "Endodontics",         Role = "Doctor"  },
						new() { Id = 38, UniversityId = zagazig, FullName = "Hassan Yasser",         Faculty = "Faculty of Dentistry", Department = "General Dentistry",   Role = "Student" },
						new() { Id = 39, UniversityId = zagazig, FullName = "Dr. Mayada Zaki",       Faculty = "Faculty of Dentistry", Department = "Restorative",         Role = "Doctor"  },
						new() { Id = 40, UniversityId = zagazig, FullName = "Menna Tarek",           Faculty = "Faculty of Dentistry", Department = "General Dentistry",   Role = "Student" },

                        // Minia University
                        new() { Id = 41, UniversityId = minia, FullName = "Dr. Osama Fathy",         Faculty = "Faculty of Dentistry", Department = "Orthodontics",        Role = "Doctor"  },
						new() { Id = 42, UniversityId = minia, FullName = "Sara Hossam",             Faculty = "Faculty of Dentistry", Department = "General Dentistry",   Role = "Student" },
						new() { Id = 43, UniversityId = minia, FullName = "Dr. Asmaa Helmy",         Faculty = "Faculty of Dentistry", Department = "Periodontics",        Role = "Doctor"  },
						new() { Id = 44, UniversityId = minia, FullName = "Amr Waleed",              Faculty = "Faculty of Dentistry", Department = "Oral Surgery",        Role = "Student" },

                        // Suez Canal University
                        new() { Id = 45, UniversityId = suez, FullName = "Dr. Magdy Rashed",         Faculty = "Faculty of Dentistry", Department = "Prosthodontics",      Role = "Doctor"  },
						new() { Id = 46, UniversityId = suez, FullName = "Nourhan Said",             Faculty = "Faculty of Dentistry", Department = "General Dentistry",   Role = "Student" },
						new() { Id = 47, UniversityId = suez, FullName = "Dr. Soha Abdallah",        Faculty = "Faculty of Dentistry", Department = "Pediatric Dentistry", Role = "Doctor"  },
						new() { Id = 48, UniversityId = suez, FullName = "Hazem Khaled",             Faculty = "Faculty of Dentistry", Department = "Endodontics",         Role = "Student" },
						new() { Id = 49, UniversityId = suez, FullName = "Dalia Ahmed",              Faculty = "Faculty of Dentistry", Department = "General Dentistry",   Role = "Student" },
						new() { Id = 50, UniversityId = suez, FullName = "Youssef Nader",            Faculty = "Faculty of Dentistry", Department = "Orthodontics",        Role = "Student" },
					};

					await context.UniversityMembers.AddRangeAsync(universityMembers);
					await context.SaveChangesAsync();
					logger.LogInformation("✅ Seeded 50 UniversityMembers.");
				}

				// ── 4. Doctors ────────────────────────────────────────────────────────
				if (!await context.Doctors.IgnoreQueryFilters().AnyAsync(d => d.Id == Guid.Parse("d0000000-0000-7000-8000-000000000001")))
				{
					// Clear existing to avoid ID mismatches with new seed set
					context.Doctors.RemoveRange(context.Doctors);
					await context.SaveChangesAsync();

					var doctors = new List<(Guid Id, User User, Doctor Doctor)>
					{
                        // Cairo University
                        (Guid.Parse("d0000000-0000-7000-8000-000000000001"), new User { Id = Guid.Parse("d0000000-0000-7000-8000-000000000001"), FullName = "Dr. Ahmed Hassan",   UserName = "ahmed.hassan",   Email = "ahmed.hassan@dentalhub.com",   EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000101" }, new Doctor(Guid.Parse("d0000000-0000-7000-8000-000000000001")) { Name = "Dr. Ahmed Hassan",   Specialty = "General Dentistry",   UniversityId = cairoId }),
						(Guid.Parse("d0000000-0000-7000-8000-000000000002"), new User { Id = Guid.Parse("d0000000-0000-7000-8000-000000000002"), FullName = "Dr. Mona Ibrahim",   UserName = "mona.ibrahim",   Email = "mona.ibrahim@dentalhub.com",   EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000102" }, new Doctor(Guid.Parse("d0000000-0000-7000-8000-000000000002")) { Name = "Dr. Mona Ibrahim",   Specialty = "Endodontics",         UniversityId = cairoId }),
						(Guid.Parse("d0000000-0000-7000-8000-000000000003"), new User { Id = Guid.Parse("d0000000-0000-7000-8000-000000000003"), FullName = "Dr. Hassan Youssef", UserName = "hassan.youssef", Email = "hassan.youssef@dentalhub.com", EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000103" }, new Doctor(Guid.Parse("d0000000-0000-7000-8000-000000000003")) { Name = "Dr. Hassan Youssef", Specialty = "Prosthodontics",      UniversityId = cairoId }),

                        // Ain Shams University
                        (Guid.Parse("d0000000-0000-7000-8000-000000000004"), new User { Id = Guid.Parse("d0000000-0000-7000-8000-000000000004"), FullName = "Dr. Sara Mohamed",   UserName = "sara.mohamed",   Email = "sara.mohamed@dentalhub.com",   EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000104" }, new Doctor(Guid.Parse("d0000000-0000-7000-8000-000000000004")) { Name = "Dr. Sara Mohamed",   Specialty = "Pediatric Dentistry", UniversityId = ainShamsId }),
						(Guid.Parse("d0000000-0000-7000-8000-000000000005"), new User { Id = Guid.Parse("d0000000-0000-7000-8000-000000000005"), FullName = "Dr. Khaled Fawzi",   UserName = "khaled.fawzi",   Email = "khaled.fawzi@dentalhub.com",   EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000105" }, new Doctor(Guid.Parse("d0000000-0000-7000-8000-000000000005")) { Name = "Dr. Khaled Fawzi",   Specialty = "Periodontics",        UniversityId = ainShamsId }),
						(Guid.Parse("d0000000-0000-7000-8000-000000000006"), new User { Id = Guid.Parse("d0000000-0000-7000-8000-000000000006"), FullName = "Dr. Amira Saad",     UserName = "amira.saad",     Email = "amira.saad@dentalhub.com",     EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000106" }, new Doctor(Guid.Parse("d0000000-0000-7000-8000-000000000006")) { Name = "Dr. Amira Saad",     Specialty = "Restorative",         UniversityId = ainShamsId }),

                        // Mansoura University
                        (Guid.Parse("d0000000-0000-7000-8000-000000000007"), new User { Id = Guid.Parse("d0000000-0000-7000-8000-000000000007"), FullName = "Dr. Hossam Naguib",  UserName = "hossam.naguib",  Email = "hossam.naguib@dentalhub.com",  EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000107" }, new Doctor(Guid.Parse("d0000000-0000-7000-8000-000000000007")) { Name = "Dr. Hossam Naguib",  Specialty = "Oral Surgery",        UniversityId = mansouraId }),
						(Guid.Parse("d0000000-0000-7000-8000-000000000008"), new User { Id = Guid.Parse("d0000000-0000-7000-8000-000000000008"), FullName = "Dr. Yasmin Ali",     UserName = "yasmin.ali",     Email = "yasmin.ali@dentalhub.com",     EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000108" }, new Doctor(Guid.Parse("d0000000-0000-7000-8000-000000000008")) { Name = "Dr. Yasmin Ali",     Specialty = "Restorative",         UniversityId = mansouraId }),

                        // Alexandria University
                        (Guid.Parse("d0000000-0000-7000-8000-000000000009"), new User { Id = Guid.Parse("d0000000-0000-7000-8000-000000000009"), FullName = "Dr. Dina Farouk",    UserName = "dina.farouk",    Email = "dina.farouk@dentalhub.com",    EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000109" }, new Doctor(Guid.Parse("d0000000-0000-7000-8000-000000000009")) { Name = "Dr. Dina Farouk",    Specialty = "Orthodontics",        UniversityId = alexId }),
						(Guid.Parse("d0000000-0000-7000-8000-000000000010"), new User { Id = Guid.Parse("d0000000-0000-7000-8000-000000000010"), FullName = "Dr. Waleed Hamdy",   UserName = "waleed.hamdy",   Email = "waleed.hamdy@dentalhub.com",   EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000110" }, new Doctor(Guid.Parse("d0000000-0000-7000-8000-000000000010")) { Name = "Dr. Waleed Hamdy",   Specialty = "Periodontics",        UniversityId = alexId }),

                        // Assiut University
                        (Guid.Parse("d0000000-0000-7000-8000-000000000011"), new User { Id = Guid.Parse("d0000000-0000-7000-8000-000000000011"), FullName = "Dr. Tarek Mostafa",  UserName = "tarek.mostafa",  Email = "tarek.mostafa@dentalhub.com",  EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000111" }, new Doctor(Guid.Parse("d0000000-0000-7000-8000-000000000011")) { Name = "Dr. Tarek Mostafa",  Specialty = "Orthodontics",        UniversityId = assiutId }),
						(Guid.Parse("d0000000-0000-7000-8000-000000000012"), new User { Id = Guid.Parse("d0000000-0000-7000-8000-000000000012"), FullName = "Dr. Noha Kamal",     UserName = "noha.kamal",     Email = "noha.kamal@dentalhub.com",     EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000112" }, new Doctor(Guid.Parse("d0000000-0000-7000-8000-000000000012")) { Name = "Dr. Noha Kamal",     Specialty = "Endodontics",         UniversityId = assiutId }),

                        // Benha University
                        (Guid.Parse("d0000000-0000-7000-8000-000000000013"), new User { Id = Guid.Parse("d0000000-0000-7000-8000-000000000013"), FullName = "Dr. Rania Adel",     UserName = "rania.adel",     Email = "rania.adel@dentalhub.com",     EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000113" }, new Doctor(Guid.Parse("d0000000-0000-7000-8000-000000000013")) { Name = "Dr. Rania Adel",     Specialty = "Oral Surgery",        UniversityId = benhaId }),
						(Guid.Parse("d0000000-0000-7000-8000-000000000014"), new User { Id = Guid.Parse("d0000000-0000-7000-8000-000000000014"), FullName = "Dr. Hisham Fouad",   UserName = "hisham.fouad",   Email = "hisham.fouad@dentalhub.com",   EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000114" }, new Doctor(Guid.Parse("d0000000-0000-7000-8000-000000000014")) { Name = "Dr. Hisham Fouad",   Specialty = "Prosthodontics",      UniversityId = benhaId }),

                        // Tanta University
                        (Guid.Parse("d0000000-0000-7000-8000-000000000015"), new User { Id = Guid.Parse("d0000000-0000-7000-8000-000000000015"), FullName = "Dr. Sherif Mohsen",  UserName = "sherif.mohsen",  Email = "sherif.mohsen@dentalhub.com",  EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000115" }, new Doctor(Guid.Parse("d0000000-0000-7000-8000-000000000015")) { Name = "Dr. Sherif Mohsen",  Specialty = "Oral Surgery",        UniversityId = tanta }),
						(Guid.Parse("d0000000-0000-7000-8000-000000000016"), new User { Id = Guid.Parse("d0000000-0000-7000-8000-000000000016"), FullName = "Dr. Eman Tawfik",    UserName = "eman.tawfik",    Email = "eman.tawfik@dentalhub.com",    EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000116" }, new Doctor(Guid.Parse("d0000000-0000-7000-8000-000000000016")) { Name = "Dr. Eman Tawfik",    Specialty = "Pediatric Dentistry", UniversityId = tanta }),

                        // Zagazig University
                        (Guid.Parse("d0000000-0000-7000-8000-000000000017"), new User { Id = Guid.Parse("d0000000-0000-7000-8000-000000000017"), FullName = "Dr. Tamer Nabil",    UserName = "tamer.nabil",    Email = "tamer.nabil@dentalhub.com",    EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000117" }, new Doctor(Guid.Parse("d0000000-0000-7000-8000-000000000017")) { Name = "Dr. Tamer Nabil",    Specialty = "Endodontics",         UniversityId = zagazig }),
						(Guid.Parse("d0000000-0000-7000-8000-000000000018"), new User { Id = Guid.Parse("d0000000-0000-7000-8000-000000000018"), FullName = "Dr. Mayada Zaki",    UserName = "mayada.zaki",    Email = "mayada.zaki@dentalhub.com",    EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000118" }, new Doctor(Guid.Parse("d0000000-0000-7000-8000-000000000018")) { Name = "Dr. Mayada Zaki",    Specialty = "Restorative",         UniversityId = zagazig }),

                        // Minia University
                        (Guid.Parse("d0000000-0000-7000-8000-000000000019"), new User { Id = Guid.Parse("d0000000-0000-7000-8000-000000000019"), FullName = "Dr. Osama Fathy",    UserName = "osama.fathy",    Email = "osama.fathy@dentalhub.com",    EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000119" }, new Doctor(Guid.Parse("d0000000-0000-7000-8000-000000000019")) { Name = "Dr. Osama Fathy",    Specialty = "Orthodontics",        UniversityId = minia }),
						(Guid.Parse("d0000000-0000-7000-8000-000000000020"), new User { Id = Guid.Parse("d0000000-0000-7000-8000-000000000020"), FullName = "Dr. Asmaa Helmy",    UserName = "asmaa.helmy",    Email = "asmaa.helmy@dentalhub.com",    EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000120" }, new Doctor(Guid.Parse("d0000000-0000-7000-8000-000000000020")) { Name = "Dr. Asmaa Helmy",    Specialty = "Periodontics",        UniversityId = minia }),

                        // Suez Canal University
                        (Guid.Parse("d0000000-0000-7000-8000-000000000021"), new User { Id = Guid.Parse("d0000000-0000-7000-8000-000000000021"), FullName = "Dr. Magdy Rashed",   UserName = "magdy.rashed",   Email = "magdy.rashed@dentalhub.com",   EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000121" }, new Doctor(Guid.Parse("d0000000-0000-7000-8000-000000000021")) { Name = "Dr. Magdy Rashed",   Specialty = "Prosthodontics",      UniversityId = suez }),
						(Guid.Parse("d0000000-0000-7000-8000-000000000022"), new User { Id = Guid.Parse("d0000000-0000-7000-8000-000000000022"), FullName = "Dr. Soha Abdallah",  UserName = "soha.abdallah",  Email = "soha.abdallah@dentalhub.com",  EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000122" }, new Doctor(Guid.Parse("d0000000-0000-7000-8000-000000000022")) { Name = "Dr. Soha Abdallah",  Specialty = "Pediatric Dentistry", UniversityId = suez }),
					};

					foreach (var (id, user, doctor) in doctors)
					{
						await CreateUserWithRoleAsync(userManager, user, "Doctor", "Doctor@123", logger);
					}

					await context.Doctors.AddRangeAsync(doctors.Select(d => d.Doctor));
					await context.SaveChangesAsync();
					logger.LogInformation("✅ Seeded 22 Doctors.");
				}

				// ── 5. Students ───────────────────────────────────────────────────────
				if (!await context.Students.IgnoreQueryFilters().AnyAsync(s => s.Id == Guid.Parse("e0000000-0000-7000-8000-000000000001")))
				{
					// Clear existing to avoid ID mismatches with new seed set
					context.Students.RemoveRange(context.Students);
					await context.SaveChangesAsync();

					var students = new List<(Guid Id, User User, Student Student)>
					{
                        // Cairo University
						(Guid.Parse("e0000000-0000-7000-8000-000000000001"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000001"), FullName = "Omar Gamal",     UserName = "omar.gamal",     Email = "omar.gamal@dentalhub.com",     EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000201" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000001")) { Level = 4, UniversityId = cairoId }),
						(Guid.Parse("e0000000-0000-7000-8000-000000000002"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000002"), FullName = "Zeyad Khaled",   UserName = "zeyad.khaled",   Email = "zeyad.khaled@dentalhub.com",   EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000202" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000002")) { Level = 3, UniversityId = cairoId }),
						(Guid.Parse("e0000000-0000-7000-8000-000000000003"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000003"), FullName = "Aya Mahmoud",    UserName = "aya.mahmoud",    Email = "aya.mahmoud@dentalhub.com",    EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000203" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000003")) { Level = 5, UniversityId = cairoId }),
						(Guid.Parse("e0000000-0000-7000-8000-000000000004"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000004"), FullName = "Mariam Fathy",   UserName = "mariam.fathy",   Email = "mariam.fathy@dentalhub.com",   EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000204" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000004")) { Level = 2, UniversityId = cairoId }),
						(Guid.Parse("e0000000-0000-7000-8000-000000000005"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000005"), FullName = "Ali Sherif",     UserName = "ali.sherif",     Email = "ali.sherif@dentalhub.com",     EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000205" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000005")) { Level = 4, UniversityId = cairoId }),

                        // Ain Shams University
                        (Guid.Parse("e0000000-0000-7000-8000-000000000006"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000006"), FullName = "Nour Ali",       UserName = "nour.ali",       Email = "nour.ali@dentalhub.com",       EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000206" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000006")) { Level = 3, UniversityId = ainShamsId }),
						(Guid.Parse("e0000000-0000-7000-8000-000000000007"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000007"), FullName = "Hana Ahmed",     UserName = "hana.ahmed",     Email = "hana.ahmed@dentalhub.com",     EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000207" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000007")) { Level = 4, UniversityId = ainShamsId }),
						(Guid.Parse("e0000000-0000-7000-8000-000000000008"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000008"), FullName = "Karim Hassan",   UserName = "karim.hassan",   Email = "karim.hassan@dentalhub.com",   EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000208" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000008")) { Level = 5, UniversityId = ainShamsId }),

                        // Mansoura University
                        (Guid.Parse("e0000000-0000-7000-8000-000000000009"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000009"), FullName = "Youssef Ibrahim", UserName = "youssef.ibrahim", Email = "youssef.ibrahim@dentalhub.com", EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000209" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000009")) { Level = 5, UniversityId = mansouraId }),
						(Guid.Parse("e0000000-0000-7000-8000-000000000010"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000010"), FullName = "Laila Mostafa",  UserName = "laila.mostafa",  Email = "laila.mostafa@dentalhub.com",  EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000210" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000010")) { Level = 3, UniversityId = mansouraId }),
						(Guid.Parse("e0000000-0000-7000-8000-000000000011"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000011"), FullName = "Mohamed Samy",   UserName = "mohamed.samy",   Email = "mohamed.samy@dentalhub.com",   EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000211" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000011")) { Level = 4, UniversityId = mansouraId }),

                        // Alexandria University
                        (Guid.Parse("e0000000-0000-7000-8000-000000000012"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000012"), FullName = "Farida Samir",   UserName = "farida.samir",   Email = "farida.samir@dentalhub.com",   EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000212" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000012")) { Level = 2, UniversityId = alexId }),
						(Guid.Parse("e0000000-0000-7000-8000-000000000013"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000013"), FullName = "Salma Adel",     UserName = "salma.adel",     Email = "salma.adel@dentalhub.com",     EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000213" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000013")) { Level = 4, UniversityId = alexId }),
						(Guid.Parse("e0000000-0000-7000-8000-000000000014"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000014"), FullName = "Yara Nabil",     UserName = "yara.nabil",     Email = "yara.nabil@dentalhub.com",     EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000214" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000014")) { Level = 3, UniversityId = alexId }),

                        // Assiut University
                        (Guid.Parse("e0000000-0000-7000-8000-000000000015"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000015"), FullName = "Mahmoud Essam",  UserName = "mahmoud.essam",  Email = "mahmoud.essam@dentalhub.com",  EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000215" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000015")) { Level = 1, UniversityId = assiutId }),
						(Guid.Parse("e0000000-0000-7000-8000-000000000016"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000016"), FullName = "Ibrahim Ashraf", UserName = "ibrahim.ashraf", Email = "ibrahim.ashraf@dentalhub.com", EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000216" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000016")) { Level = 5, UniversityId = assiutId }),

                        // Benha University
                        (Guid.Parse("e0000000-0000-7000-8000-000000000017"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000017"), FullName = "Salma Wael",     UserName = "salma.wael",     Email = "salma.wael@dentalhub.com",     EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000217" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000017")) { Level = 4, UniversityId = benhaId }),
						(Guid.Parse("e0000000-0000-7000-8000-000000000018"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000018"), FullName = "Nada Omar",      UserName = "nada.omar",      Email = "nada.omar@dentalhub.com",      EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000218" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000018")) { Level = 2, UniversityId = benhaId }),

                        // Tanta University
                        (Guid.Parse("e0000000-0000-7000-8000-000000000019"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000019"), FullName = "Ahmed Saber",    UserName = "ahmed.saber",    Email = "ahmed.saber@dentalhub.com",    EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000219" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000019")) { Level = 3, UniversityId = tanta }),
						(Guid.Parse("e0000000-0000-7000-8000-000000000020"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000020"), FullName = "Rana Magdy",     UserName = "rana.magdy",     Email = "rana.magdy@dentalhub.com",     EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000220" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000020")) { Level = 4, UniversityId = tanta }),

                        // Zagazig University
                        (Guid.Parse("e0000000-0000-7000-8000-000000000021"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000021"), FullName = "Hassan Yasser",  UserName = "hassan.yasser",  Email = "hassan.yasser@dentalhub.com",  EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000221" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000021")) { Level = 5, UniversityId = zagazig }),
						(Guid.Parse("e0000000-0000-7000-8000-000000000022"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000022"), FullName = "Menna Tarek",    UserName = "menna.tarek",    Email = "menna.tarek@dentalhub.com",    EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000222" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000022")) { Level = 3, UniversityId = zagazig }),

                        // Minia University
                        (Guid.Parse("e0000000-0000-7000-8000-000000000023"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000023"), FullName = "Sara Hossam",    UserName = "sara.hossam",    Email = "sara.hossam@dentalhub.com",    EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000223" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000023")) { Level = 2, UniversityId = minia }),
						(Guid.Parse("e0000000-0000-7000-8000-000000000024"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000024"), FullName = "Amr Waleed",     UserName = "amr.waleed",     Email = "amr.waleed@dentalhub.com",     EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000224" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000024")) { Level = 4, UniversityId = minia }),

                        // Suez Canal University
                        (Guid.Parse("e0000000-0000-7000-8000-000000000025"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000025"), FullName = "Nourhan Said",   UserName = "nourhan.said",   Email = "nourhan.said@dentalhub.com",   EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000225" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000025")) { Level = 3, UniversityId = suez }),
						(Guid.Parse("e0000000-0000-7000-8000-000000000026"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000026"), FullName = "Hazem Khaled",   UserName = "hazem.khaled",   Email = "hazem.khaled@dentalhub.com",   EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000226" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000026")) { Level = 4, UniversityId = suez }),
						(Guid.Parse("e0000000-0000-7000-8000-000000000027"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000027"), FullName = "Dalia Ahmed",    UserName = "dalia.ahmed",    Email = "dalia.ahmed@dentalhub.com",   EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000227" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000027")) { Level = 5, UniversityId = suez }),
						(Guid.Parse("e0000000-0000-7000-8000-000000000028"), new User { Id = Guid.Parse("e0000000-0000-7000-8000-000000000028"), FullName = "Youssef Nader",  UserName = "youssef.nader",  Email = "youssef.nader@dentalhub.com",  EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000228" }, new Student(Guid.Parse("e0000000-0000-7000-8000-000000000028")) { Level = 2, UniversityId = suez }),
					};

					foreach (var (id, user, student) in students)
					{
						await CreateUserWithRoleAsync(userManager, user, "Student", "Student@123", logger);
					}

					await context.Students.AddRangeAsync(students.Select(s => s.Student));
					await context.SaveChangesAsync();
					logger.LogInformation("✅ Seeded 28 Students.");
				}

				// ── 6. Patients ───────────────────────────────────────────────────────
				if (!await context.Patients.IgnoreQueryFilters().AnyAsync(p => p.Id == Guid.Parse("f0000000-0000-7000-8000-000000000001")))
				{
					// Clear existing to avoid ID mismatches with new seed set
					context.Patients.RemoveRange(context.Patients);
					await context.SaveChangesAsync();

					var patients = new List<(Guid Id, User User, Patient Patient)>
					{
						(Guid.Parse("f0000000-0000-7000-8000-000000000001"), new User { Id = Guid.Parse("f0000000-0000-7000-8000-000000000001"), FullName = "Mona Tarek",      UserName = "01000000301", Email = "mona.tarek@gmail.com",      EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000301" }, new Patient(Guid.Parse("f0000000-0000-7000-8000-000000000001")) { Age = 30, Phone = "01000000301", Gender = Gender.Female }),
						(Guid.Parse("f0000000-0000-7000-8000-000000000002"), new User { Id = Guid.Parse("f0000000-0000-7000-8000-000000000002"), FullName = "Karim Salah",     UserName = "01000000302", Email = "karim.salah@gmail.com",     EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000302" }, new Patient(Guid.Parse("f0000000-0000-7000-8000-000000000002")) { Age = 45, Phone = "01000000302", Gender = Gender.Male }),
						(Guid.Parse("f0000000-0000-7000-8000-000000000003"), new User { Id = Guid.Parse("f0000000-0000-7000-8000-000000000003"), FullName = "Layla Khaled",    UserName = "01000000303", Email = "layla.khaled@gmail.com",    EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000303" }, new Patient(Guid.Parse("f0000000-0000-7000-8000-000000000003")) { Age = 25, Phone = "01000000303", Gender = Gender.Female }),
						(Guid.Parse("f0000000-0000-7000-8000-000000000004"), new User { Id = Guid.Parse("f0000000-0000-7000-8000-000000000004"), FullName = "Hany Salem",      UserName = "01000000304", Email = "hany.salem@gmail.com",      EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000304" }, new Patient(Guid.Parse("f0000000-0000-7000-8000-000000000004")) { Age = 52, Phone = "01000000304", Gender = Gender.Male }),
						(Guid.Parse("f0000000-0000-7000-8000-000000000005"), new User { Id = Guid.Parse("f0000000-0000-7000-8000-000000000005"), FullName = "Amina Youssef",   UserName = "01000000305", Email = "amina.youssef@gmail.com",   EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000305" }, new Patient(Guid.Parse("f0000000-0000-7000-8000-000000000005")) { Age = 38, Phone = "01000000305", Gender = Gender.Female }),
						(Guid.Parse("f0000000-0000-7000-8000-000000000006"), new User { Id = Guid.Parse("f0000000-0000-7000-8000-000000000006"), FullName = "Tarek Fahmy",     UserName = "01000000306", Email = "tarek.fahmy@gmail.com",     EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000306" }, new Patient(Guid.Parse("f0000000-0000-7000-8000-000000000006")) { Age = 28, Phone = "01000000306", Gender = Gender.Male }),
						(Guid.Parse("f0000000-0000-7000-8000-000000000007"), new User { Id = Guid.Parse("f0000000-0000-7000-8000-000000000007"), FullName = "Noura Hassan",    UserName = "01000000307", Email = "noura.hassan@gmail.com",    EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000307" }, new Patient(Guid.Parse("f0000000-0000-7000-8000-000000000007")) { Age = 41, Phone = "01000000307", Gender = Gender.Female }),
						(Guid.Parse("f0000000-0000-7000-8000-000000000008"), new User { Id = Guid.Parse("f0000000-0000-7000-8000-000000000008"), FullName = "Sherif Nabil",    UserName = "01000000308", Email = "sherif.nabil@gmail.com",    EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000308" }, new Patient(Guid.Parse("f0000000-0000-7000-8000-000000000008")) { Age = 35, Phone = "01000000308", Gender = Gender.Male }),
						(Guid.Parse("f0000000-0000-7000-8000-000000000009"), new User { Id = Guid.Parse("f0000000-0000-7000-8000-000000000009"), FullName = "Dina Adel",       UserName = "01000000309", Email = "dina.adel@gmail.com",       EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000309" }, new Patient(Guid.Parse("f0000000-0000-7000-8000-000000000009")) { Age = 29, Phone = "01000000309", Gender = Gender.Female }),
						(Guid.Parse("f0000000-0000-7000-8000-000000000010"), new User { Id = Guid.Parse("f0000000-0000-7000-8000-000000000010"), FullName = "Yasser Ahmed",    UserName = "01000000310", Email = "yasser.ahmed@gmail.com",    EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000310" }, new Patient(Guid.Parse("f0000000-0000-7000-8000-000000000010")) { Age = 50, Phone = "01000000310", Gender = Gender.Male }),
						(Guid.Parse("f0000000-0000-7000-8000-000000000011"), new User { Id = Guid.Parse("f0000000-0000-7000-8000-000000000011"), FullName = "Rana Samir",      UserName = "01000000311", Email = "rana.samir@gmail.com",      EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000311" }, new Patient(Guid.Parse("f0000000-0000-7000-8000-000000000011")) { Age = 33, Phone = "01000000311", Gender = Gender.Female }),
						(Guid.Parse("f0000000-0000-7000-8000-000000000012"), new User { Id = Guid.Parse("f0000000-0000-7000-8000-000000000012"), FullName = "Mahmoud Fathy",   UserName = "01000000312", Email = "mahmoud.fathy@gmail.com",   EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000312" }, new Patient(Guid.Parse("f0000000-0000-7000-8000-000000000012")) { Age = 47, Phone = "01000000312", Gender = Gender.Male }),
						(Guid.Parse("f0000000-0000-7000-8000-000000000013"), new User { Id = Guid.Parse("f0000000-0000-7000-8000-000000000013"), FullName = "Salma Ibrahim",   UserName = "01000000313", Email = "salma.ibrahim@gmail.com",   EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000313" }, new Patient(Guid.Parse("f0000000-0000-7000-8000-000000000013")) { Age = 26, Phone = "01000000313", Gender = Gender.Female }),
						(Guid.Parse("f0000000-0000-7000-8000-000000000014"), new User { Id = Guid.Parse("f0000000-0000-7000-8000-000000000014"), FullName = "Khaled Mostafa",  UserName = "01000000314", Email = "khaled.mostafa@gmail.com",  EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000314" }, new Patient(Guid.Parse("f0000000-0000-7000-8000-000000000014")) { Age = 39, Phone = "01000000314", Gender = Gender.Male }),
						(Guid.Parse("f0000000-0000-7000-8000-000000000015"), new User { Id = Guid.Parse("f0000000-0000-7000-8000-000000000015"), FullName = "Eman Ali",        UserName = "01000000315", Email = "eman.ali@gmail.com",        EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000315" }, new Patient(Guid.Parse("f0000000-0000-7000-8000-000000000015")) { Age = 31, Phone = "01000000315", Gender = Gender.Female }),
					};

					foreach (var (id, user, patient) in patients)
					{
						await CreateUserWithRoleAsync(userManager, user, "Patient", "Patient@123", logger);
					}

					await context.Patients.AddRangeAsync(patients.Select(p => p.Patient));
					await context.SaveChangesAsync();
					logger.LogInformation("✅ Seeded 15 Patients.");
				}

				// ── 7. Admins ─────────────────────────────────────────────────────────
				if (!await context.Admins.IgnoreQueryFilters().AnyAsync(a => a.Id == Guid.Parse("a0000000-0000-7000-8000-000000000001")))
				{
					// Clear existing to avoid ID mismatches
					context.Admins.RemoveRange(context.Admins);
					await context.SaveChangesAsync();

					var adminId = Guid.Parse("a0000000-0000-7000-8000-000000000001");
					var adminUser = new User { Id = adminId, FullName = "System Admin", UserName = "admin", Email = "admin@dentalhub.com", EmailConfirmed = true, PhoneNumberConfirmed = true, PhoneNumber = "01000000401" };
					var admin = new Admin(adminId) { Phone = "01000000401", IsSuperAdmin = true, UniversityId = cairoId };

					await CreateUserWithRoleAsync(userManager, adminUser, "Admin", "Admin@123", logger);
					await context.Admins.AddAsync(admin);
					await context.SaveChangesAsync();
					logger.LogInformation("✅ Seeded Admin Account.");
				}

				// ── 8. Case Types ─────────────────────────────────────────────────────
				var typeOrtho = Guid.Parse("01960000-0000-7000-8000-000000000100");
				var typeRoot = Guid.Parse("01960000-0000-7000-8000-000000000101");
				var typeExt = Guid.Parse("01960000-0000-7000-8000-000000000102");
				var typePediat = Guid.Parse("01960000-0000-7000-8000-000000000103");
				var typeScaling = Guid.Parse("01960000-0000-7000-8000-000000000104");
				var typeImplant = Guid.Parse("01960000-0000-7000-8000-000000000105");
				var typeFilling = Guid.Parse("01960000-0000-7000-8000-000000000106");
				var typeCrown = Guid.Parse("01960000-0000-7000-8000-000000000107");
				var typeVeneer = Guid.Parse("01960000-0000-7000-8000-000000000108");
				var typeWhitening = Guid.Parse("01960000-0000-7000-8000-000000000109");

				if (!await context.CaseTypes.AnyAsync(ct => ct.Id == typeOrtho) || !await context.CaseTypes.AnyAsync(ct => ct.Id == typeWhitening))
				{
					// Clear existing to avoid ID mismatches
					context.CaseTypes.RemoveRange(context.CaseTypes);
					await context.SaveChangesAsync();

					var caseTypes = new List<CaseType>
					{
						new() { Id = typeOrtho,     Name = "Orthodontics",         Description = "Braces, aligners, and jaw correction procedures." },
						new() { Id = typeRoot,      Name = "Root Canal",           Description = "Endodontic therapy to treat infection at the centre of a tooth." },
						new() { Id = typeExt,       Name = "Tooth Extraction",     Description = "Removal of a tooth from its socket in the bone." },
						new() { Id = typePediat,    Name = "Pediatric Dentistry",  Description = "Oral health care for children from infancy through the teen years." },
						new() { Id = typeScaling,   Name = "Scaling and Polishing", Description = "Deep cleaning to remove plaque and tartar buildup." },
						new() { Id = typeImplant,   Name = "Dental Implants",      Description = "Surgical component that interfaces with the bone of the jaw or skull." },
						new() { Id = typeFilling,   Name = "Fillings",             Description = "Restoration of lost tooth structure using materials such as composite or amalgam." },
						new() { Id = typeCrown,     Name = "Dental Crowns",        Description = "Tooth-shaped cap placed over a damaged tooth to restore its shape and strength." },
						new() { Id = typeVeneer,    Name = "Veneers",              Description = "Thin shells of porcelain or composite resin bonded to the front of teeth." },
						new() { Id = typeWhitening, Name = "Teeth Whitening",      Description = "Cosmetic procedure to lighten teeth and remove stains and discoloration." }
					};
					await context.CaseTypes.AddRangeAsync(caseTypes);
					await context.SaveChangesAsync();
					logger.LogInformation("✅ Seeded 10 Case Types.");
				}

				// ── 9. Patient Cases ──────────────────────────────────────────────────
				if (!await context.PatientCases.AnyAsync())
				{
					var patientCases = new List<PatientCase>();

					// Helper IDs
					var pat1 = Guid.Parse("f0000000-0000-7000-8000-000000000001"); // Mona
					var pat2 = Guid.Parse("f0000000-0000-7000-8000-000000000002"); // Karim
					var pat3 = Guid.Parse("f0000000-0000-7000-8000-000000000003"); // Layla
					var pat4 = Guid.Parse("f0000000-0000-7000-8000-000000000004"); // Hany
					var pat5 = Guid.Parse("f0000000-0000-7000-8000-000000000005"); // Amina
					var pat6 = Guid.Parse("f0000000-0000-7000-8000-000000000006"); // Tarek
					var pat7 = Guid.Parse("f0000000-0000-7000-8000-000000000007"); // Noura
					var pat8 = Guid.Parse("f0000000-0000-7000-8000-000000000008"); // Sherif
					var pat9 = Guid.Parse("f0000000-0000-7000-8000-000000000009"); // Dina
					var pat10 = Guid.Parse("f0000000-0000-7000-8000-000000000010"); // Yasser
					var pat11 = Guid.Parse("f0000000-0000-7000-8000-000000000011"); // Rana
					var pat12 = Guid.Parse("f0000000-0000-7000-8000-000000000012"); // Mahmoud
					var pat13 = Guid.Parse("f0000000-0000-7000-8000-000000000013"); // Salma
					var pat14 = Guid.Parse("f0000000-0000-7000-8000-000000000014"); // Khaled
					var pat15 = Guid.Parse("f0000000-0000-7000-8000-000000000015"); // Eman

					var stu1 = Guid.Parse("e0000000-0000-7000-8000-000000000001"); // Omar
					var stu6 = Guid.Parse("e0000000-0000-7000-8000-000000000006"); // Nour
					var stu9 = Guid.Parse("e0000000-0000-7000-8000-000000000009"); // Youssef
					var stu12 = Guid.Parse("e0000000-0000-7000-8000-000000000012"); // Farida

					var doc1 = Guid.Parse("d0000000-0000-7000-8000-000000000001"); // Ahmed
					var doc2 = Guid.Parse("d0000000-0000-7000-8000-000000000002"); // Mona Ibrahim
					var doc4 = Guid.Parse("d0000000-0000-7000-8000-000000000004"); // Sara
					var doc7 = Guid.Parse("d0000000-0000-7000-8000-000000000007"); // Hossam
					var doc9 = Guid.Parse("d0000000-0000-7000-8000-000000000009"); // Dina Farouk

					// Generate 30+ cases with variety
					patientCases.AddRange(new[]
					{
                        // InProgress Cases
                        new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat1, Status = CaseStatus.InProgress, Description = "Root canal treatment on upper right molar.", AssignedStudentId = stu1, AssignedDoctorId = doc1, UniversityId = cairoId, IsPublic = true, CreatedByRole = "Patient", CreatedById = pat1 },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat2, Status = CaseStatus.InProgress, Description = "Composite filling for lower left molar cavity.", AssignedStudentId = stu6, AssignedDoctorId = doc4, UniversityId = ainShamsId, IsPublic = true, CreatedByRole = "Patient", CreatedById = pat2 },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat5, Status = CaseStatus.InProgress, Description = "Orthodontic braces adjustment and monitoring.", AssignedStudentId = stu12, AssignedDoctorId = doc9, UniversityId = alexId, IsPublic = false, CreatedByRole = "Doctor", CreatedById = doc9 },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat8, Status = CaseStatus.InProgress, Description = "Crown preparation for upper front tooth.", AssignedStudentId = stu9, AssignedDoctorId = doc7, UniversityId = mansouraId, IsPublic = true, CreatedByRole = "Patient", CreatedById = pat8 },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat11, Status = CaseStatus.InProgress, Description = "Deep scaling and root planing treatment.", AssignedStudentId = stu1, AssignedDoctorId = doc2, UniversityId = cairoId, IsPublic = true, CreatedByRole = "Patient", CreatedById = pat11 },

                        // Pending Cases
                        new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat3, Status = CaseStatus.Pending, Description = "Orthodontic evaluation for crowded teeth.", UniversityId = cairoId, IsPublic = true, CreatedByRole = "Patient", CreatedById = pat3 },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat4, Status = CaseStatus.Pending, Description = "Patient interested in braces for aesthetic reasons.", UniversityId = cairoId, IsPublic = true, CreatedByRole = "Patient", CreatedById = pat4 },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat6, Status = CaseStatus.Pending, Description = "Wisdom tooth extraction consultation needed.", UniversityId = ainShamsId, IsPublic = false, CreatedByRole = "Patient", CreatedById = pat6 },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat7, Status = CaseStatus.Pending, Description = "Routine cleaning and checkup.", UniversityId = ainShamsId, IsPublic = true, CreatedByRole = "Patient", CreatedById = pat7 },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat9, Status = CaseStatus.Pending, Description = "Teeth whitening consultation for discolored teeth.", UniversityId = mansouraId, IsPublic = true, CreatedByRole = "Patient", CreatedById = pat9 },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat10, Status = CaseStatus.Pending, Description = "Dental implant evaluation for missing tooth.", UniversityId = alexId, IsPublic = true, CreatedByRole = "Patient", CreatedById = pat10 },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat12, Status = CaseStatus.Pending, Description = "Veneer consultation for chipped front tooth.", UniversityId = benhaId, IsPublic = false, CreatedByRole = "Patient", CreatedById = pat12 },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat13, Status = CaseStatus.Pending, Description = "Pediatric dental checkup for 8-year-old child.", UniversityId = tanta, IsPublic = true, CreatedByRole = "Patient", CreatedById = pat13 },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat14, Status = CaseStatus.Pending, Description = "Emergency toothache requiring immediate attention.", UniversityId = zagazig, IsPublic = true, CreatedByRole = "Patient", CreatedById = pat14 },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat15, Status = CaseStatus.Pending, Description = "Gum disease treatment consultation.", UniversityId = minia, IsPublic = true, CreatedByRole = "Patient", CreatedById = pat15 },

                        // Completed Cases
                        new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat1, Status = CaseStatus.Completed, Description = "Lower left premolar extracted successfully.", AssignedStudentId = stu1, AssignedDoctorId = doc1, UniversityId = cairoId, IsPublic = true, CreatedByRole = "Patient", CreatedById = pat1 },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat2, Status = CaseStatus.Completed, Description = "Scaling and polishing completed.", AssignedStudentId = stu6, AssignedDoctorId = doc4, UniversityId = ainShamsId, IsPublic = true, CreatedByRole = "Patient", CreatedById = pat2 },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat3, Status = CaseStatus.Completed, Description = "Root canal treatment finished successfully.", AssignedStudentId = stu1, AssignedDoctorId = doc2, UniversityId = cairoId, IsPublic = false, CreatedByRole = "Doctor", CreatedById = doc2 },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat5, Status = CaseStatus.Completed, Description = "Two composite fillings placed in upper molars.", AssignedStudentId = stu12, AssignedDoctorId = doc9, UniversityId = alexId, IsPublic = true, CreatedByRole = "Patient", CreatedById = pat5 },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat7, Status = CaseStatus.Completed, Description = "Crown and bridge work completed.", AssignedStudentId = stu9, AssignedDoctorId = doc7, UniversityId = mansouraId, IsPublic = true, CreatedByRole = "Patient", CreatedById = pat7 },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat10, Status = CaseStatus.Completed, Description = "Wisdom teeth extraction (all four) performed.", AssignedStudentId = stu1, AssignedDoctorId = doc1, UniversityId = cairoId, IsPublic = true, CreatedByRole = "Patient", CreatedById = pat10 },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat12, Status = CaseStatus.Completed, Description = "Orthodontic treatment completed after 18 months.", AssignedStudentId = stu12, AssignedDoctorId = doc9, UniversityId = alexId, IsPublic = false, CreatedByRole = "Doctor", CreatedById = doc9 },

                        // UnderReview Cases (AI preliminary)
                        new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat3, Status = CaseStatus.UnderReview, Description = "Automated scan detection of potential gingivitis.", UniversityId = null, IsPublic = true, CreatedByRole = "AI", CreatedById = null },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat6, Status = CaseStatus.UnderReview, Description = "AI detected early signs of tooth decay on X-ray.", UniversityId = null, IsPublic = true, CreatedByRole = "AI", CreatedById = null },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat9, Status = CaseStatus.UnderReview, Description = "Automated detection of potential periodontal disease.", UniversityId = null, IsPublic = true, CreatedByRole = "AI", CreatedById = null },

                        // Rejected Cases
                        new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat4, Status = CaseStatus.Rejected, Description = "Cosmetic veneer request - patient budget constraints.", UniversityId = cairoId, IsPublic = false, CreatedByRole = "Patient", CreatedById = pat4 },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat8, Status = CaseStatus.Rejected, Description = "Implant case - patient medical history not suitable.", UniversityId = mansouraId, IsPublic = false, CreatedByRole = "Doctor", CreatedById = doc7 },

                        // Additional variety
                        new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat11, Status = CaseStatus.Pending, Description = "Second opinion requested for complex root canal.", UniversityId = benhaId, IsPublic = true, CreatedByRole = "Patient", CreatedById = pat11 },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat13, Status = CaseStatus.InProgress, Description = "Braces installed, monthly adjustment ongoing.", AssignedStudentId = stu12, AssignedDoctorId = doc9, UniversityId = alexId, IsPublic = true, CreatedByRole = "Student", CreatedById = stu12 },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat14, Status = CaseStatus.Completed, Description = "Emergency filling completed same day.", AssignedStudentId = stu6, AssignedDoctorId = doc4, UniversityId = ainShamsId, IsPublic = true, CreatedByRole = "Patient", CreatedById = pat14 },
						new PatientCase { Id = Guid.CreateVersion7(), PatientId = pat15, Status = CaseStatus.Pending, Description = "Full mouth rehabilitation consultation required.", UniversityId = minia, IsPublic = false, CreatedByRole = "Patient", CreatedById = pat15 },
					});

					await context.PatientCases.AddRangeAsync(patientCases);
					await context.SaveChangesAsync();
					logger.LogInformation($"✅ Seeded {patientCases.Count} Patient Cases.");
				}

				// ── 10. Diagnoses ─────────────────────────────────────────────────────
				if (!await context.Diagnoses.AnyAsync())
				{
					var allCases = await context.PatientCases.Take(15).ToListAsync();
					var diagnoses = new List<Diagnosis>();

					foreach (var patientCase in allCases)
					{
						// Add AI preliminary diagnosis for most cases
						if (patientCase.Status != CaseStatus.Rejected)
						{
							diagnoses.Add(new Diagnosis
							{
								Id = Guid.CreateVersion7(),
								PatientCaseId = patientCase.Id,
								CaseTypeId = GetRandomCaseType(),
								Stage = DiagnosisStage.AI,
								Notes = "AI preliminary scan completed. Anomalies detected and flagged for review.",
								CreatedById = null,
								Role = "System",
								IsAccepted = patientCase.Status == CaseStatus.InProgress || patientCase.Status == CaseStatus.Completed ? true : null,
								TeethNumbers = GetRandomTeeth()
							});
						}

						// Add BasicClinic diagnosis for InProgress and Completed
						if (patientCase.Status == CaseStatus.InProgress || patientCase.Status == CaseStatus.Completed)
						{
							diagnoses.Add(new Diagnosis
							{
								Id = Guid.CreateVersion7(),
								PatientCaseId = patientCase.Id,
								CaseTypeId = GetRandomCaseType(),
								Stage = DiagnosisStage.BasicClinic,
								Notes = "Clinical examination confirms diagnosis. Treatment plan approved.",
								CreatedById = patientCase.AssignedDoctorId,
								Role = "Doctor",
								IsAccepted = true,
								TeethNumbers = GetRandomTeeth()
							});
						}

						// Add AdvancedClinic diagnosis for Completed
						if (patientCase.Status == CaseStatus.Completed)
						{
							diagnoses.Add(new Diagnosis
							{
								Id = Guid.CreateVersion7(),
								PatientCaseId = patientCase.Id,
								CaseTypeId = GetRandomCaseType(),
								Stage = DiagnosisStage.AdvancedClinic,
								Notes = "Treatment completed successfully. Post-operative examination normal.",
								CreatedById = patientCase.AssignedDoctorId,
								Role = "Doctor",
								IsAccepted = true,
								TeethNumbers = GetRandomTeeth()
							});
						}
					}

					await context.Diagnoses.AddRangeAsync(diagnoses);
					await context.SaveChangesAsync();
					logger.LogInformation($"✅ Seeded {diagnoses.Count} Diagnoses.");
				}

				// ── 11. Sessions & Session Notes ──────────────────────────────────────
				if (!await context.Sessions.AnyAsync())
				{
					var completedCases = await context.PatientCases
						.Where(c => c.Status == CaseStatus.Completed || c.Status == CaseStatus.InProgress)
						.Take(10)
						.ToListAsync();

					var sessions = new List<Session>();
					var sessionNotes = new List<SessionNote>();

					foreach (var patientCase in completedCases)
					{
						// Create 2-4 sessions per case
						var sessionCount = Random.Shared.Next(2, 5);
						for (int i = 0; i < sessionCount; i++)
						{
							var sessionId = Guid.CreateVersion7();
							var isDone = patientCase.Status == CaseStatus.Completed || (patientCase.Status == CaseStatus.InProgress && i < sessionCount - 1);

							var session = new Session
							{
								Id = sessionId,
								CaseId = patientCase.Id,
								StudentId = patientCase.AssignedStudentId!.Value,
								PatientId = patientCase.PatientId,
								StartAt = DateTime.UtcNow.AddDays(-30 + (i * 7)),
								EndAt = DateTime.UtcNow.AddDays(-30 + (i * 7)).AddHours(1.5),
								Status = isDone ? SessionStatus.Done : SessionStatus.Scheduled,
								Grade = isDone ? Random.Shared.Next(75, 96) : 0,
								DoctorNote = isDone ? $"Session {i + 1} completed. Good progress." : string.Empty,
								EvaluteDoctorId = isDone ? patientCase.AssignedDoctorId : null
							};
							sessions.Add(session);

							// Add notes for done sessions
							if (isDone)
							{
								sessionNotes.Add(new SessionNote
								{
									Id = Guid.CreateVersion7(),
									SessionId = sessionId,
									Note = $"Patient arrived on time. Session {i + 1} proceeded as planned."
								});
								sessionNotes.Add(new SessionNote
								{
									Id = Guid.CreateVersion7(),
									SessionId = sessionId,
									Note = "Treatment performed according to protocol. Patient tolerated procedure well."
								});
							}
						}
					}

					await context.Sessions.AddRangeAsync(sessions);
					await context.SessionNotes.AddRangeAsync(sessionNotes);
					await context.SaveChangesAsync();
					logger.LogInformation($"✅ Seeded {sessions.Count} Sessions and {sessionNotes.Count} Session Notes.");
				}

				// ── 12. Case Requests ─────────────────────────────────────────────────
				if (!await context.CaseRequests.AnyAsync())
				{
					var pendingCases = await context.PatientCases
						.Where(c => c.Status == CaseStatus.Pending)
						.Take(10)
						.ToListAsync();

					var students = await context.Students.IgnoreQueryFilters().Take(15).ToListAsync();
					var doctors = await context.Doctors.IgnoreQueryFilters().Take(10).ToListAsync();

					var caseRequests = new List<CaseRequest>();

					foreach (var pendingCase in pendingCases)
					{
						// Create 2-3 requests per pending case
						var requestCount = Random.Shared.Next(2, 4);
						for (int i = 0; i < requestCount; i++)
						{
							var student = students[Random.Shared.Next(students.Count)];
							var doctor = doctors[Random.Shared.Next(doctors.Count)];

							caseRequests.Add(new CaseRequest
							{
								Id = Guid.CreateVersion7(),
								StudentId = student.Id,
								DoctorId = doctor.Id,
								PatientCaseId = pendingCase.Id,
								Description = $"Requesting supervision for this case. I have relevant experience in this area.",
								Status = i == 0 ? RequestStatus.Pending : (RequestStatus)Random.Shared.Next(0, 3)
							});
						}
					}

					await context.CaseRequests.AddRangeAsync(caseRequests);
					await context.SaveChangesAsync();
					logger.LogInformation($"✅ Seeded {caseRequests.Count} Case Requests.");
				}
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "❌ An error occurred while seeding the database.");
				throw;
			}
		}

		// ── Helpers ───────────────────────────────────────────────────────────────
		private static async Task CreateUserWithRoleAsync(
			UserManager<User> userManager,
			User user,
			string role,
			string password,
			ILogger logger)
		{
			var existingById = await userManager.FindByIdAsync(user.Id.ToString());
			if (existingById != null) return;

			var existingByEmail = await userManager.FindByEmailAsync(user.Email!);
			if (existingByEmail != null)
			{
				// If ID changed but email is same, we must remove the old user to avoid conflicts
				await userManager.DeleteAsync(existingByEmail);
				logger.LogWarning("🗑️ Removed stale user with email {Email} to update ID.", user.Email);
			}

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

		private static Guid GetRandomCaseType()
		{
			var caseTypes = new[]
			{
				Guid.Parse("01960000-0000-7000-8000-000000000100"), // Orthodontics
                Guid.Parse("01960000-0000-7000-8000-000000000101"), // Root Canal
                Guid.Parse("01960000-0000-7000-8000-000000000102"), // Extraction
                Guid.Parse("01960000-0000-7000-8000-000000000103"), // Pediatric
                Guid.Parse("01960000-0000-7000-8000-000000000104"), // Scaling
                Guid.Parse("01960000-0000-7000-8000-000000000105"), // Implant
                Guid.Parse("01960000-0000-7000-8000-000000000106"), // Filling
            };
			return caseTypes[Random.Shared.Next(caseTypes.Length)];
		}

		private static List<int> GetRandomTeeth()
		{
			var count = Random.Shared.Next(1, 4);
			var teeth = new List<int>();
			for (int i = 0; i < count; i++)
			{
				teeth.Add(Random.Shared.Next(1, 33));
			}
			return teeth.Distinct().ToList();
		}
	}
}