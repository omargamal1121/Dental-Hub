using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Admins;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.Specification;
using DentalHub.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace DentalHub.Application.Services.Admins
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly ILogger<AdminService> _logger;

        public AdminService(
            IUnitOfWork unitOfWork,
            UserManager<User> userManager,
            RoleManager<IdentityRole<Guid>> roleManager,
            ILogger<AdminService> logger)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
        }

        #region Admin Registration

        /// Register a new admin
        public async Task<Result<AdminDto>> RegisterAdminAsync(RegisterAdminDto dto)
        {
            try
            {
                // STEP 1: Check if email already exists
                var existingUser = await _userManager.FindByEmailAsync(dto.Email);
                if (existingUser != null)
                {
                    return Result<AdminDto>.Failure("Email already exists");
                }

                // STEP 2: Create User in Identity System
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
                    return Result<AdminDto>.Failure(errors);
                }

                // STEP 3: Add Admin Role
                await EnsureRoleExistsAsync("Admin");
                await _userManager.AddToRoleAsync(user, "Admin");

                // STEP 4: Create Admin Record
                var admin = new Admin
                {
                    UserId = user.Id,
                    Role = dto.Role,
                    Phone = dto.Phone,
                    IsSuperAdmin = dto.IsSuperAdmin,
                    CreateAt = DateTime.UtcNow
                };

                await _unitOfWork.Admins.AddAsync(admin);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Admin created successfully: {Email}", dto.Email);

                // STEP 5: Return Response
                return await GetAdminByIdAsync(user.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering admin: {Email}", dto.Email);
                return Result<AdminDto>.Failure("An error occurred during registration");
            }
        }

        #endregion

        #region Admin Profile

        /// Get admin by user ID
        public async Task<Result<AdminDto>> GetAdminByIdAsync(Guid userId)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Admin, AdminDto>(
                    a => a.UserId == userId,
                    a => new AdminDto
                    {
                        UserId = a.UserId,
                        FullName = a.User.FullName,
                        Email = a.User.Email!,
                        Role = a.Role,
                        Phone = a.Phone,
                        IsSuperAdmin = a.IsSuperAdmin,
                        CreateAt = a.CreateAt
                    }
                );

                spec.AddInclude(a => a.User);

                var admin = await _unitOfWork.Admins.GetByIdAsync(spec);

                if (admin == null)
                {
                    return Result<AdminDto>.Failure("Admin not found");
                }

                return Result<AdminDto>.Success(admin);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin: {UserId}", userId);
                return Result<AdminDto>.Failure("Error retrieving admin data");
            }
        }

        /// Get all admins with pagination
        public async Task<Result<List<AdminDto>>> GetAllAdminsAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Admin, AdminDto>(
                    a => new AdminDto
                    {
                        UserId = a.UserId,
                        FullName = a.User.FullName,
                        Email = a.User.Email!,
                        Role = a.Role,
                        Phone = a.Phone,
                        IsSuperAdmin = a.IsSuperAdmin,
                        CreateAt = a.CreateAt
                    }
                );

                spec.AddInclude(a => a.User);
                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderByDescending(a => a.CreateAt);

                var admins = await _unitOfWork.Admins.GetAllAsync(spec);

                return Result<List<AdminDto>>.Success(admins);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all admins");
                return Result<List<AdminDto>>.Failure("Error retrieving admins");
            }
        }

        /// Get admins by role
        public async Task<Result<List<AdminDto>>> GetAdminsByRoleAsync(
            string role, int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Admin, AdminDto>(
                    a => a.Role == role,
                    a => new AdminDto
                    {
                        UserId = a.UserId,
                        FullName = a.User.FullName,
                        Email = a.User.Email!,
                        Role = a.Role,
                        Phone = a.Phone,
                        IsSuperAdmin = a.IsSuperAdmin,
                        CreateAt = a.CreateAt
                    }
                );

                spec.AddInclude(a => a.User);
                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderByDescending(a => a.CreateAt);

                var admins = await _unitOfWork.Admins.GetAllAsync(spec);

                return Result<List<AdminDto>>.Success(admins);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admins by role: {Role}", role);
                return Result<List<AdminDto>>.Failure("Error retrieving admins");
            }
        }

        /// Get super admins only
        public async Task<Result<List<AdminDto>>> GetSuperAdminsAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Admin, AdminDto>(
                    a => a.IsSuperAdmin == true,
                    a => new AdminDto
                    {
                        UserId = a.UserId,
                        FullName = a.User.FullName,
                        Email = a.User.Email!,
                        Role = a.Role,
                        Phone = a.Phone,
                        IsSuperAdmin = a.IsSuperAdmin,
                        CreateAt = a.CreateAt
                    }
                );

                spec.AddInclude(a => a.User);
                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderByDescending(a => a.CreateAt);

                var admins = await _unitOfWork.Admins.GetAllAsync(spec);

                return Result<List<AdminDto>>.Success(admins);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting super admins");
                return Result<List<AdminDto>>.Failure("Error retrieving super admins");
            }
        }

        /// Update admin information
        public async Task<Result<AdminDto>> UpdateAdminAsync(UpdateAdminDto dto)
        {
            try
            {
                var spec = new BaseSpecification<Admin>(a => a.UserId == dto.UserId);
                spec.AddInclude(a => a.User);

                var admin = await _unitOfWork.Admins.GetByIdAsync(spec);

                if (admin == null)
                {
                    return Result<AdminDto>.Failure("Admin not found");
                }

                // Update fields if provided
                if (!string.IsNullOrWhiteSpace(dto.FullName))
                {
                    admin.User.FullName = dto.FullName;
                }

                if (!string.IsNullOrWhiteSpace(dto.Role))
                {
                    admin.Role = dto.Role;
                }

                if (!string.IsNullOrWhiteSpace(dto.Phone))
                {
                    admin.Phone = dto.Phone;
                    admin.User.PhoneNumber = dto.Phone;
                }

                if (dto.IsSuperAdmin.HasValue)
                {
                    admin.IsSuperAdmin = dto.IsSuperAdmin.Value;
                }

                admin.UpdateAt = DateTime.UtcNow;

                _unitOfWork.Admins.Update(admin);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Admin updated successfully: {UserId}", dto.UserId);

                return await GetAdminByIdAsync(dto.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating admin: {UserId}", dto.UserId);
                return Result<AdminDto>.Failure("Error updating admin");
            }
        }

        /// Soft delete admin
        public async Task<Result> DeleteAdminAsync(Guid userId)
        {
            try
            {
                var admin = await _unitOfWork.Admins.GetByIdAsync(
                    new BaseSpecification<Admin>(a => a.UserId == userId));

                if (admin == null)
                {
                    return Result.Failure("Admin not found");
                }

                admin.DeleteAt = DateTime.UtcNow;
                _unitOfWork.Admins.Update(admin);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Admin deleted: {UserId}", userId);

                return Result.Success("Admin deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting admin: {UserId}", userId);
                return Result.Failure("Error deleting admin");
            }
        }

        #endregion

        #region Statistics

        /// Get admin statistics
        /// System-wide statistics visible to admin
        public async Task<Result<AdminStatsDto>> GetAdminStatisticsAsync(Guid adminId)
        {
            try
            {
                var spec = new BaseSpecification<Admin>(a => a.UserId == adminId);
                var admin = await _unitOfWork.Admins.GetByIdAsync(spec);

                if (admin == null)
                {
                    return Result<AdminStatsDto>.Failure("Admin not found");
                }

                // Get all users count
                var usersSpec = new BaseSpecification<User>(_ => true);
                var totalUsers = await _unitOfWork.Users.CountAsync(usersSpec);

                // Get all patients count
                var patientsSpec = new BaseSpecification<Patient>(_ => true);
                var totalPatients = await _unitOfWork.Patients.CountAsync(patientsSpec);

                // Get all students count
                var studentsSpec = new BaseSpecification<Student>(_ => true);
                var totalStudents = await _unitOfWork.Students.CountAsync(studentsSpec);

                // Get all doctors count
                var doctorsSpec = new BaseSpecification<Doctor>(_ => true);
                var totalDoctors = await _unitOfWork.Doctors.CountAsync(doctorsSpec);

                // Get all cases
                var casesSpec = new BaseSpecification<PatientCase>(_ => true);
                var totalCases = await _unitOfWork.PatientCases.CountAsync(casesSpec);

                // Get active cases
                var activeCasesSpec = new BaseSpecification<PatientCase>(
                    c => c.Status == CaseStatus.InProgress);
                var activeCases = await _unitOfWork.PatientCases.CountAsync(activeCasesSpec);

                // Get completed cases
                var completedCasesSpec = new BaseSpecification<PatientCase>(
                    c => c.Status == CaseStatus.Completed);
                var completedCases = await _unitOfWork.PatientCases.CountAsync(completedCasesSpec);

                // Get all sessions
                var sessionsSpec = new BaseSpecification<Session>(_ => true);
                var totalSessions = await _unitOfWork.Sessions.CountAsync(sessionsSpec);

                var stats = new AdminStatsDto
                {
                    TotalUsers = totalUsers,
                    TotalPatients = totalPatients,
                    TotalStudents = totalStudents,
                    TotalDoctors = totalDoctors,
                    TotalCases = totalCases,
                    ActiveCases = activeCases,
                    CompletedCases = completedCases,
                    TotalSessions = totalSessions
                };

                return Result<AdminStatsDto>.Success(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting admin statistics: {AdminId}", adminId);
                return Result<AdminStatsDto>.Failure("Error retrieving statistics");
            }
        }

        #endregion

        #region Helper Methods

        /// Ensure role exists in database
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
