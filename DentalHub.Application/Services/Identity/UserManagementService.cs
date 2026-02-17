using Microsoft.EntityFrameworkCore;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Identity;
using DentalHub.Application.Exceptions;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DentalHub.Application.Services.Identity
{
  
    public class UserManagementService : IUserManagementService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserManagementService> _logger;

        public UserManagementService(
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IUnitOfWork unitOfWork,
            ILogger<UserManagementService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region Patient Registration

 
        public async Task<Result<AuthResponseDto>> RegisterPatientAsync(RegisterPatientDto dto)
        {
            try
            {
              
                var existingUser = await _userManager.FindByEmailAsync(dto.Email);
                if (existingUser != null)
                {
                    return Result<AuthResponseDto>.Failure("Email already exists");
                }

                var user = new User
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FullName = dto.FullName,
                    PhoneNumber = dto.Phone,
                    EmailConfirmed = true 
                };

               
                var result = await _userManager.CreateAsync(user, dto.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return Result<AuthResponseDto>.Failure(errors);
                }

  
                await EnsureRoleExistsAsync("Patient");
                await _userManager.AddToRoleAsync(user, "Patient");

            
                var patient = new Patient
                {
                    
                    UserId = user.Id, 
                    Age = dto.Age,
                    Phone = dto.Phone,
                    CreateAt = DateTime.UtcNow
                };
                patient.Id=user.Id;

                await _unitOfWork.Patients.AddAsync(patient);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Patient registered successfully: {Email}", dto.Email);

                return Result<AuthResponseDto>.Success(new AuthResponseDto
                {
                    PublicId = user.PublicId,
                    Email = user.Email!,
                    FullName = user.FullName,
                    Role = "Patient"
                }, "Registration successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering patient: {Email}", dto.Email);
                return Result<AuthResponseDto>.Failure("An error occurred during registration");
            }
        }

        #endregion

        #region Student Registration

        
        public async Task<Result<AuthResponseDto>> RegisterStudentAsync(RegisterStudentDto dto)
        {
            try
            {
               
                var existingUser = await _userManager.FindByEmailAsync(dto.Email);
                if (existingUser != null)
                {
                    return Result<AuthResponseDto>.Failure("Email already exists");
                }

              
                var user = new User
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FullName = dto.FullName,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, dto.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return Result<AuthResponseDto>.Failure(errors);
                }
                await EnsureRoleExistsAsync("Student");
                await _userManager.AddToRoleAsync(user, "Student");

                var student = new Student
                {
                    UserId = user.Id,
                    University = dto.University,
                    UniversityId = dto.UniversityId,
                    Level = dto.Level,
                    CreateAt = DateTime.UtcNow
                };
                student.Id=user.Id;
                await _unitOfWork.Students.AddAsync(student);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Student registered successfully: {Email}", dto.Email);

                return Result<AuthResponseDto>.Success(new AuthResponseDto
                {
                    PublicId = user.PublicId,
                    Email = user.Email!,
                    FullName = user.FullName,
                    Role = "Student"
                }, "Registration successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering student: {Email}", dto.Email);
                return Result<AuthResponseDto>.Failure("An error occurred during registration");
            }
        }

        #endregion

        #region Doctor Registration

        /// Register a new doctor
        public async Task<Result<AuthResponseDto>> RegisterDoctorAsync(RegisterDoctorDto dto)
        {
            try
            {
                // STEP 1: Check email
                var existingUser = await _userManager.FindByEmailAsync(dto.Email);
                if (existingUser != null)
                {
                    return Result<AuthResponseDto>.Failure("Email already exists");
                }

                // STEP 2: Create User
                var user = new User
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FullName = dto.FullName,
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(user, dto.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return Result<AuthResponseDto>.Failure(errors);
                }

                // STEP 3: Add Role
                await EnsureRoleExistsAsync("Doctor");
                await _userManager.AddToRoleAsync(user, "Doctor");

                // STEP 4: Create Doctor Record
                var doctor = new Doctor
                {
                    UserId = user.Id,
                    Name = dto.Name,
                    Specialty = dto.Specialty,
                    UniversityId = dto.UniversityId,
                    CreateAt = DateTime.UtcNow
                };
                doctor.Id = user.Id;
                await _unitOfWork.Doctors.AddAsync(doctor);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Doctor registered successfully: {Email}", dto.Email);

                return Result<AuthResponseDto>.Success(new AuthResponseDto
                {
                    PublicId = user.PublicId,
                    Email = user.Email!,
                    FullName = user.FullName,
                    Role = "Doctor"
                }, "Registration successful");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering doctor: {Email}", dto.Email);
                return Result<AuthResponseDto>.Failure("An error occurred during registration");
            }
        }

        #endregion

        #region Helper Methods

 
        public async Task<Result<bool>> CheckEmailExistsAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                return Result<bool>.Success(user != null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking email: {Email}", email);
                return Result<bool>.Failure("Error checking email");
            }
        }

    
        public async Task<Result> DeleteUserAsync(string publicId)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.PublicId == publicId);
                if (user == null)
                {
                    return Result.Failure("User not found");
                }

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return Result.Failure(errors);
                }

                return Result.Success("User deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with public ID: {PublicId}", publicId);
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
