using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Identity;
using DentalHub.Application.Exceptions;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DentalHub.Application.Services.Identity
{
    /// <summary>
    /// Authentication service - handles user registration, login, and management
    /// هنا بيتم التعامل مع UserManager لإدارة المستخدمين
    /// </summary>
    public class AuthService : IUserManagementService
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            IUnitOfWork unitOfWork,
            ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region Patient Registration

        /// <summary>
        /// Register a new patient
        /// خطوات التسجيل:
        /// 1. التحقق إن الإيميل مش مكرر
        /// 2. إنشاء User في الـ Identity System
        /// 3. إضافة Role للـ User
        /// 4. إنشاء Patient Record في البيزنس جداول
        /// </summary>
        public async Task<Result<AuthResponseDto>> RegisterPatientAsync(RegisterPatientDto dto)
        {
            try
            {
                // STEP 1: Check if email already exists
                var existingUser = await _userManager.FindByEmailAsync(dto.Email);
                if (existingUser != null)
                {
                    return Result<AuthResponseDto>.Failure("Email already exists");
                }

                // STEP 2: Create User in Identity System
                var user = new User
                {
                    UserName = dto.Email,
                    Email = dto.Email,
                    FullName = dto.FullName,
                    PhoneNumber = dto.Phone,
                    EmailConfirmed = true // للتبسيط، في الإنتاج هنحتاج Email Confirmation
                };

                // هنا بنحفظ الـ User مع الـ Password في جداول الـ Identity
                var result = await _userManager.CreateAsync(user, dto.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return Result<AuthResponseDto>.Failure(errors);
                }

                // STEP 3: Add Role to User
                await EnsureRoleExistsAsync("Patient");
                await _userManager.AddToRoleAsync(user, "Patient");

                // STEP 4: Create Patient Record
                var patient = new Patient
                {
                    UserId = user.Id, // Foreign Key من الـ Identity User
                    Age = dto.Age,
                    Phone = dto.Phone,
                    CreateAt = DateTime.UtcNow
                };

                await _unitOfWork.Patients.AddAsync(patient);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Patient registered successfully: {Email}", dto.Email);

                // STEP 5: Return Response
                return Result<AuthResponseDto>.Success(new AuthResponseDto
                {
                    UserId = user.Id,
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

        /// <summary>
        /// Register a new student
        /// نفس خطوات الـ Patient لكن بنضيف بيانات Student
        /// </summary>
        public async Task<Result<AuthResponseDto>> RegisterStudentAsync(RegisterStudentDto dto)
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
                await EnsureRoleExistsAsync("Student");
                await _userManager.AddToRoleAsync(user, "Student");

                // STEP 4: Create Student Record
                var student = new Student
                {
                    UserId = user.Id,
                    University = dto.University,
                    UniversityId = dto.UniversityId,
                    Level = dto.Level,
                    CreateAt = DateTime.UtcNow
                };

                await _unitOfWork.Students.AddAsync(student);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Student registered successfully: {Email}", dto.Email);

                return Result<AuthResponseDto>.Success(new AuthResponseDto
                {
                    UserId = user.Id,
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

        /// <summary>
        /// Register a new doctor
        /// </summary>
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

                await _unitOfWork.Doctors.AddAsync(doctor);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Doctor registered successfully: {Email}", dto.Email);

                return Result<AuthResponseDto>.Success(new AuthResponseDto
                {
                    UserId = user.Id,
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

        /// <summary>
        /// Check if email already exists
        /// </summary>
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

        /// <summary>
        /// Delete user and all related data
        /// </summary>
        public async Task<Result> DeleteUserAsync(Guid userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
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
                _logger.LogError(ex, "Error deleting user: {UserId}", userId);
                return Result.Failure("An error occurred while deleting user");
            }
        }

        /// <summary>
        /// Ensure role exists in database
        /// بنتأكد إن الـ Role موجود، لو مش موجود بننشئه
        /// </summary>
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
