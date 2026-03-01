using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Identity;
using DentalHub.Application.Exceptions;
using DentalHub.Application.Specification.Comman;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data.Entity;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

                var today = DateTime.Today;
				var age = today.Year - dto.BirthDate.Year;
				if (dto.BirthDate.Date > today.AddYears(-age))
					age--;

                if (age < 5||age>100)
                { 
                  
				    return Result<AuthResponseDto>.Failure("Invalid Age Must Be Greater than 5 years and less than 100",400);

				}

				await _unitOfWork.BeginTransactionAsync();

	
				var userName = dto.Phone;

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

	
			

				var patient = new Patient(user.Id,user.PublicId)
				{
					Age = age,
					Phone = dto.Phone,
					CreateAt = DateTime.UtcNow,
                    
				};

				await _unitOfWork.Patients.AddAsync(patient);
				await _unitOfWork.SaveChangesAsync();

				
				await _unitOfWork.CommitTransactionAsync();

				_logger.LogInformation("Patient registered successfully: {Phone}", dto.Phone);

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

				var student = new Student(user.Id,user.PublicId)
                {
                    Level = dto.Level,
				
                    University = dto.University,
                    UniversityId = dto.UniversityId,
				
                };
             
                await _unitOfWork.Students.AddAsync(student);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

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

				var doctor = new Doctor(user.Id,user.PublicId)
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
                    PublicId = user.PublicId,
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
