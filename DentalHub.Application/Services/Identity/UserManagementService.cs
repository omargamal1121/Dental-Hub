using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Identity;
using DentalHub.Application.Exceptions;
using DentalHub.Application.Services.Doctors;
using DentalHub.Application.Services.Students;
using DentalHub.Application.Services;
using DentalHub.Application.Specification.Comman;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging;
using DentalHub.Application.DTOs.Doctors;

namespace DentalHub.Application.Services.Identity
{
  
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserManagementService> _logger;
        private readonly IPatientService _patientService;
        private readonly IDoctorService _doctorService;
        private readonly IStudentService _studentService;

		public UserManagementService(
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IUnitOfWork unitOfWork,
            ILogger<UserManagementService> logger,
            IPatientService patientService,
            IDoctorService doctorService,
            IStudentService studentService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _patientService = patientService;
            _doctorService = doctorService;
            _studentService = studentService;
        }



		#region Student Registration


		public async Task<Result<AuthResponseDto>> RegisterStudentAsync(RegisterStudentDto dto)
        {
            try
            {
				var spec = new BaseSpecification<UniversityMember>(u => u.UniversityId == dto.UniversityId && u.Role == "Student");
				if (!await _unitOfWork.UniversityMembers.AnyAsync(spec))
				{
					return Result<AuthResponseDto>.Failure("Invalid University ID or University does not have a Student role", 400);
				}

				await _unitOfWork.BeginTransactionAsync();

				var user = new User
                {
                    PhoneNumber = dto.Phone,
                    
					UserName = dto.Username,
                    Email = dto.Email,
                    FullName = dto.FullName,
                    EmailConfirmed = true,
                    
                    
                };

                var result = await _userManager.CreateAsync(user, dto.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                   await  _unitOfWork.RollbackTransactionAsync();
					return Result<AuthResponseDto>.Failure(errors);
                }
              
                 var roleResult=   await _userManager.AddToRoleAsync(user, "Student");

				if (!roleResult.Succeeded)
				{
					await _unitOfWork.RollbackTransactionAsync();
					var errors = roleResult.Errors.Select(e => e.Description).ToList();
					return Result<AuthResponseDto>.Failure(errors);
				}

				var student = new Student(user.Id)
                {
                    Level = dto.Level,
                    UniversityId = dto.UniversityId,
				
                };
             
                await _unitOfWork.Students.AddAsync(student);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

				_logger.LogInformation("Student registered successfully: {Email}", dto.Email);

                return Result<AuthResponseDto>.Success(new AuthResponseDto
                {
                    PublicId = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName,
                    Role = "Student"
                }, "Registration successful");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error registering student: {Email}", dto.Email);
                return Result<AuthResponseDto>.Failure("An error occurred during registration");
            }
        }

        #endregion

        #region Doctor Registration

     
        public async Task<Result<AuthResponseDto>> RegisterDoctorAsync(RegisterDoctorDto dto)
        {
            try
            {
                var spec= new BaseSpecification<UniversityMember>(u=>u.UniversityId==dto.UniversityId&&u.Role=="Doctor");
				if(! await _unitOfWork.UniversityMembers.AnyAsync(spec))
                {
                    return Result<AuthResponseDto>.Failure("Invalid University ID or University does not have a Doctor role",400);
				}
				await _unitOfWork.BeginTransactionAsync();

				var user = new User

                {  
                    PhoneNumber=dto.Phone,
					UserName = dto.Username,
                    Email = dto.Email,
                    FullName = dto.FullName,
                    EmailConfirmed = true,
                    
                };

                var result = await _userManager.CreateAsync(user, dto.Password);

                if (!result.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
					var errors = result.Errors.Select(e => e.Description).ToList();
                    return Result<AuthResponseDto>.Failure(errors);
                }

      
               var roleResult= await _userManager.AddToRoleAsync(user, "Doctor");

				if (!roleResult.Succeeded)
				{
					await _unitOfWork.RollbackTransactionAsync();
					var errors = roleResult.Errors.Select(e => e.Description).ToList();
					return Result<AuthResponseDto>.Failure(errors);
				}

				var doctor = new Doctor(user.Id)
                {
                    
                    Name = dto.FullName,
                    Specialty = dto.Specialty,
                    UniversityId = dto.UniversityId,
                    CreateAt = DateTime.UtcNow,
                    
                };
              
                await _unitOfWork.Doctors.AddAsync(doctor);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

				_logger.LogInformation("Doctor registered successfully: {Email}", dto.Email);

                return Result<AuthResponseDto>.Success(new AuthResponseDto
                {
                    PublicId = user.Id,
                    Email = user.Email!,
                    FullName = user.FullName,
                    Role = "Doctor"
                }, "Registration successful");
            }
            catch (Exception ex)
            {
               await  _unitOfWork.RollbackTransactionAsync();
				_logger.LogError(ex, "Error registering doctor: {Email}", dto.Email);
                return Result<AuthResponseDto>.Failure("An error occurred during registration");
            }
        }

		#endregion

		#region Helper Methods





		public async Task<Result> DeleteUserAsync(Guid userId)
		{
			try
			{
				await _unitOfWork.BeginTransactionAsync();

				var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
				if (user == null)
					return Result.Failure("User not found", 404);

				var roles = await _userManager.GetRolesAsync(user);
				var role = roles.FirstOrDefault() ?? "Unknown";

				Result result = role switch
				{
					"Patient" => await _patientService.HandleBeforeDeleteAsync(user.Id),
					"Doctor" => await _doctorService.HandleBeforeDeleteAsync(user.Id),
					"Student" => await _studentService.HandleBeforeDeleteAsync(user.Id),
					_ => Result.Success()
				};

				if (!result.IsSuccess)
				{
					await _unitOfWork.RollbackTransactionAsync();
					return Result.Failure(result.Message ?? "Failed to delete user", result.Status);
				}

				user.DeletedAt = DateTime.UtcNow;
				user.IsDeleted = true;
				var deleteResult = await _userManager.UpdateAsync(user);

				if (!deleteResult.Succeeded)
				{
					await _unitOfWork.RollbackTransactionAsync();
					return Result.Failure("Failed to update user for soft delete");
				}

				await _unitOfWork.SaveChangesAsync();
				await _unitOfWork.CommitTransactionAsync();

				_logger.LogInformation("User deleted successfully: {UserId}", userId);
				return Result.Success("User deleted successfully",200);
			}
			catch (Exception ex)
			{
				await _unitOfWork.RollbackTransactionAsync();
				_logger.LogError(ex, "Error deleting user with Id: {UserId}", userId);
				return Result.Failure("An error occurred while deleting user");
			}
		}
		public async Task<Result<List<DoctorlistDto>>> GetClinicalDoctorsAsync()
		{
			try
			{
				var clinicalDoctors = await _userManager.GetUsersInRoleAsync("ClinicalDoctor");
				var result = new List<DoctorlistDto>();

				foreach (var u in clinicalDoctors)
				{
					var universityId = Guid.Empty;
					var specialty = "Clinical Diagnosis Specialist";
					var createdAt = DateTime.UtcNow;

					// Try Doctor table
					var doctorData = await _unitOfWork.Doctors.GetByIdAsync(new BaseSpecificationWithProjection<Doctor, (Guid UniversityId, string Specialty, DateTime CreateAt)>(
						d => d.Id == u.Id,
						d =>new  (d.UniversityId, d.Specialty, d.CreateAt)
					));

					if (doctorData.UniversityId != Guid.Empty)
					{
						universityId = doctorData.UniversityId;
						specialty = doctorData.Specialty;
						createdAt = doctorData.CreateAt;
					}
					else
					{
						// Try Student table
						universityId = await _unitOfWork.Students.GetByIdAsync(new BaseSpecificationWithProjection<Student, Guid>(
							s => s.Id == u.Id,
							s => s.UniversityId
						));

						if (universityId == Guid.Empty)
						{
							// Try Admin table
							universityId = await _unitOfWork.Admins.GetByIdAsync(new BaseSpecificationWithProjection<Admin, Guid>(
								a => a.Id == u.Id,
								a => a.UniversityId
							));
						}
					}

					result.Add(new DoctorlistDto
					{
						PublicId = u.Id,
						FullName = u.FullName,
						Email = u.Email ?? string.Empty,
						Username = u.UserName ?? string.Empty,
						Name = u.FullName,
						Specialty = specialty,
						UniversityId = universityId,
						CreateAt = createdAt
					});
				}

				return Result<List<DoctorlistDto>>.Success(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error getting clinical doctors");
				return Result<List<DoctorlistDto>>.Failure("Error retrieving clinical doctors");
			}
		}

		private async Task EnsureRoleExistsAsync(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
            }
        }

        #endregion
    }
}
