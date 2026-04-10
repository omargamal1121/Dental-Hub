using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Patients;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.UnitOfWork;
using DentalHub.Application.Factories;
using Microsoft.Extensions.Logging;
using DentalHub.Application.Specification.Comman;
using DentalHub.Application.DTOs.Cases;

namespace DentalHub.Application.Services
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

        public async Task<Result<PatientDto>> GetPatientByIdAsync(Guid id)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Patient, PatientDto>(
                    p => p.Id == id,
                    p => new PatientDto
                    {
                        PublicId = p.Id,
                        FullName = p.User.FullName,
                        Email = p.User.Email!,
                        Phone = p.Phone,
                        Age = p.Age,
                        CreateAt = p.CreateAt,
                        PatientCases = p.PatientCases
                            .Select(pc => new PatientCaseSimpleDataDto
                            {
                                Id = pc.Id,
                           //     Name = pc.CaseType.Name,
                                Status = pc.Status,
                                CreateAt = pc.CreateAt
                            })
                            .ToList()
                    }
                );

                var patient = await _unitOfWork.Patients.GetByIdAsync(spec);

                if (patient == null)
                    return Result<PatientDto>.Failure("Patient not found");

                return Result<PatientDto>.Success(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient by ID: {Id}", id);
                return Result<PatientDto>.Failure("Error retrieving patient data");
            }
        }


        public async Task<Result<PatientDto>> GetPatientByUserIdAsync(Guid userId)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Patient, PatientDto>(
                    p => p.Id == userId,
                    p => new PatientDto
                    {
                        PublicId = p.Id,
                        FullName = p.User.FullName,
                        Email = p.User.Email!,
                        Phone = p.Phone,
                        Age = p.Age,
                        CreateAt = p.CreateAt,
                        PatientCases = p.PatientCases
                            .Select(pc => new PatientCaseSimpleDataDto
                            {
                                Id = pc.Id,
                              //  Name = pc.CaseType.Name,
                                Status = pc.Status,
                                CreateAt = pc.CreateAt
                            })
                            .ToList()
                    }
                );

                var patient = await _unitOfWork.Patients.GetByIdAsync(spec);

                if (patient == null)
                    return Result<PatientDto>.Failure("Patient not found");

                return Result<PatientDto>.Success(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting patient by user ID: {UserId}", userId);
                return Result<PatientDto>.Failure("Error retrieving patient data");
            }
        }

        public async Task<Result<PagedResult<PatientDto>>> GetAllPatientsAsync(FilterPatientDto filterPatientDto, int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Patient, PatientDto>(
                    p => filterPatientDto.Name == null || p.User.FullName.Contains(filterPatientDto.Name),
                    p => new PatientDto
                    {
                        PublicId = p.Id,
                        FullName = p.User.FullName,
                        Email = p.User.Email!,
                        Phone = p.Phone,
                        Age = p.Age,
                        CreateAt = p.CreateAt,

                        PatientCases = p.PatientCases
                            .Where(pc => filterPatientDto.CaseStatus == null || pc.Status == filterPatientDto.CaseStatus.Value)
                            .Where(pc => string.IsNullOrEmpty(filterPatientDto.CaseType) 
                          //  || pc.CaseType.Name.Contains(filterPatientDto.CaseType!)
                            )
                            .Select(pc => new PatientCaseSimpleDataDto
                            {
                                Id = pc.Id,
                             //   Name = pc.CaseType.Name,
                                Status = pc.Status,
                                CreateAt = pc.CreateAt
                            })
                            .ToList()
                    }
                );

                spec.AddInclude(p => p.User);
                spec.AddInclude(p => p.PatientCases);
                spec.AddInclude("PatientCases.CaseType");
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

        public async Task<Result<PatientDto>> UpdatePatientAsync(UpdatePatientDto dto)
        {
            try
            {
                var spec = new BaseSpecification<Patient>(p => p.Id == dto.PublicId);
                spec.AddInclude(p => p.User);

                var patient = await _unitOfWork.Patients.GetByIdAsync(spec);

                if (patient == null)
                    return Result<PatientDto>.Failure("Patient not found");

                if (!string.IsNullOrWhiteSpace(dto.FullName))
                    patient.User.FullName = dto.FullName;

                if (!string.IsNullOrWhiteSpace(dto.Phone))
                {
                    patient.Phone = dto.Phone;
                    patient.User.PhoneNumber = dto.Phone;
                }

                if (dto.Age.HasValue)
                    patient.Age = dto.Age.Value;

                patient.UpdateAt = DateTime.UtcNow;

                _unitOfWork.Patients.Update(patient);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Patient updated successfully: {Id}", dto.PublicId);


                return Result<PatientDto>.Success(new PatientDto
                {
                    PublicId = patient.Id,
                    FullName = patient.User.FullName,
                    Email = patient.User.Email!,
                    Phone = patient.Phone,
                    Age = patient.Age,
                    CreateAt = patient.CreateAt,

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient: {PublicId}", dto.PublicId);
                return Result<PatientDto>.Failure("Error updating patient");
            }
        }

        public async Task<Result> HandleBeforeDeleteAsync(Guid id)
        {

            try
            {
                _logger.LogInformation("Attempting to delete patient: {Id}", id);
				var patient = await _unitOfWork.Patients.GetByIdAsync(
                    new BaseSpecificationWithProjection<Patient, GetPatientDataById>(p =>  p.Id == id,p=>
                    new GetPatientDataById
					{
                        Id = p.Id,
                        HasProgressCases = p.PatientCases.Any(pc => pc.Status == CaseStatus.InProgress)
					}));

                if (patient == null)
                    return Result.Failure("Patient not found");
                if (patient.HasProgressCases)
                    return Result.Failure("Cannot delete patient with in-progress cases");
                
                 await _unitOfWork.PatientCases.UpdatePatientCasesStatusAsync(patient.Id, CaseStatus.Cancelled);
                await _unitOfWork.CaseRequests.CancelPendingRequestsForPatientAsync(patient.Id);

				
                await _unitOfWork.SaveChangesAsync();
				_logger.LogInformation("Patient deleted: {Id}", id);
                return Result.Success("Patient deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting patient: {Id}", id);
				return Result.Failure("Error deleting patient");
            }
        }
    }
    public class GetPatientDataById
	{
        public Guid Id { get; set; }
		public bool HasProgressCases { get; set; }
	}
}
