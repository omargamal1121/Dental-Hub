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

		#region Patient Registration


		public async Task<Result<AuthResponseDto>> RegisterPatientAsync(RegisterPatientDto dto)
		{
			try
			{

                var today = DateTime.Today;
				var age = today.Year - dto.BirthDate.Year;
				if (dto.BirthDate.Date > today.AddYears(-age))
					age--;

                if (age < 5||age>100)
                { 
                  
				    return Result<AuthResponseDto>.Failure("Invalid Age Must Be Greater than 5 years and less than 100",400);

				}

				await _unitOfWork.BeginTransactionAsync();

				var userName = !string.IsNullOrWhiteSpace(dto.UserName) ? dto.UserName : dto.Phone;

				var user = new User
				{
					UserName = userName,
					Email = dto.Email,
					FullName = dto.FullName,
					PhoneNumber = dto.Phone,
                    PhoneNumberConfirmed=true,
				
				};

				var result = await _userManager.CreateAsync(user, dto.Password);
				if (!result.Succeeded)
				{
					await _unitOfWork.RollbackTransactionAsync();
					var errors = result.Errors.Select(e => e.Description).ToList();
					return Result<AuthResponseDto>.Failure(errors);
				}

		
				var roleResult = await _userManager.AddToRoleAsync(user, "Patient");
				if (!roleResult.Succeeded)
				{
					await _unitOfWork.RollbackTransactionAsync();
					var errors = roleResult.Errors.Select(e => e.Description).ToList();
					return Result<AuthResponseDto>.Failure(errors);
				}

	
			

				var patient = new Patient(user.Id)
				{
					Age = age,
					Phone = dto.Phone,
					CreateAt = DateTime.UtcNow,
					Gender = dto.Gender
				};

				await _unitOfWork.Patients.AddAsync(patient);
				await _unitOfWork.SaveChangesAsync();

				
				await _unitOfWork.CommitTransactionAsync();

				_logger.LogInformation("Patient registered successfully: {Phone}", dto.Phone);

                return Result<AuthResponseDto>.Success(new AuthResponseDto
				{
					PublicId = user.Id,
					Email = user.Email!,
					FullName = user.FullName,
					Role = "Patient"
				}, "Registration successful");
			}
			catch (Exception ex)
			{
				await _unitOfWork.RollbackTransactionAsync();
				_logger.LogError(ex, "Error registering patient: {Phone}", dto.Phone);
				return Result<AuthResponseDto>.Failure("An error occurred during registration");
			}
		}
		#endregion

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
