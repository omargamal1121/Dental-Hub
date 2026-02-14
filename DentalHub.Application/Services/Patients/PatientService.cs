using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Patients;
using DentalHub.Application.Exceptions;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.Specification;
using DentalHub.Infrastructure.UnitOfWork;
using DentalHub.Application.Factories;
using Microsoft.Extensions.Logging;

namespace DentalHub.Application.Services.Patients
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PatientService> _logger;

        public PatientService(IUnitOfWork unitOfWork, ILogger<PatientService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        /// Get patient by user ID with projection to DTO
        public async Task<Result<PatientDto>> GetPatientByIdAsync(Guid userId)
        {
            try
            {
                // Create specification with projection
                var spec = new BaseSpecificationWithProjection<Patient, PatientDto>(
                    p => p.UserId == userId,
                    p => new PatientDto
                    {
                        UserId = p.UserId,
                        FullName = p.User.FullName,
                        Email = p.User.Email!,
                        Phone = p.Phone,
                        Age = p.Age,
                        CreateAt = p.CreateAt,
                        TotalCases = p.PatientCases.Count,
                        ActiveCases = p.PatientCases.Count(c => c.Status == CaseStatus.InProgress)
                    }
                );

                // Add includes for User navigation
                spec.AddInclude(p => p.User);
                spec.AddInclude(p => p.PatientCases);

                var patient = await _unitOfWork.Patients.GetByIdAsync(spec);

                if (patient == null)
                {
                    return Result<PatientDto>.Failure("Patient not found");
                }

                return Result<PatientDto>.Success(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient: {UserId}", userId);
                return Result<PatientDto>.Failure("Error retrieving patient data");
            }
        }

        /// Get all patients with pagination
        public async Task<Result<PagedResult<PatientDto>>> GetAllPatientsAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Patient, PatientDto>(
                    p => new PatientDto
                    {
                        UserId = p.UserId,
                        FullName = p.User.FullName,
                        Email = p.User.Email!,
                        Phone = p.Phone,
                        Age = p.Age,
                        CreateAt = p.CreateAt,
                        TotalCases = p.PatientCases.Count,
                        ActiveCases = p.PatientCases.Count(c => c.Status == CaseStatus.InProgress)
                    }
                );

                spec.AddInclude(p => p.User);
                spec.AddInclude(p => p.PatientCases);
                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderByDescending(p => p.CreateAt);

				var patientsList = await _unitOfWork.Patients.GetAllAsync(spec);
				var totalCount = await _unitOfWork.Patients.CountAsync(spec);


				var pagedResult = PaginationFactory<PatientDto>.Create(
					count: totalCount,
					page: page,
					pageSize: pageSize,
					data: patientsList
				);

                return Result<PagedResult<PatientDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all patients");
                return Result<PagedResult<PatientDto>>.Failure("Error retrieving patients");
            }
        }

        /// Update patient information
        public async Task<Result<PatientDto>> UpdatePatientAsync(UpdatePatientDto dto)
        {
            try
            {
                // Get patient
                var spec = new BaseSpecification<Patient>(p => p.UserId == dto.UserId);
                spec.AddInclude(p => p.User);

                var patient = await _unitOfWork.Patients.GetByIdAsync(spec);

                if (patient == null)
                {
                    return Result<PatientDto>.Failure("Patient not found");
                }

                // Update fields if provided
                if (!string.IsNullOrWhiteSpace(dto.FullName))
                {
                    patient.User.FullName = dto.FullName;
                }

                if (!string.IsNullOrWhiteSpace(dto.Phone))
                {
                    patient.Phone = dto.Phone;
                    patient.User.PhoneNumber = dto.Phone;
                }

                if (dto.Age.HasValue)
                {
                    patient.Age = dto.Age.Value;
                }

                patient.UpdateAt = DateTime.UtcNow;

                _unitOfWork.Patients.Update(patient);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Patient updated successfully: {UserId}", dto.UserId);

                // Return updated patient
                return await GetPatientByIdAsync(dto.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient: {UserId}", dto.UserId);
                return Result<PatientDto>.Failure("Error updating patient");
            }
        }

        /// Soft delete patient (set DeleteAt)
        public async Task<Result> DeletePatientAsync(Guid userId)
        {
            try
            {
                var patient = await _unitOfWork.Patients.GetByIdAsync(
                    new BaseSpecification<Patient>(p => p.UserId == userId));

                if (patient == null)
                {
                    return Result.Failure("Patient not found");
                }

                // Soft delete
                patient.DeleteAt = DateTime.UtcNow;
                _unitOfWork.Patients.Update(patient);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Patient deleted: {UserId}", userId);

                return Result.Success("Patient deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting patient: {UserId}", userId);
                return Result.Failure("Error deleting patient");
            }
        }
    }
}
