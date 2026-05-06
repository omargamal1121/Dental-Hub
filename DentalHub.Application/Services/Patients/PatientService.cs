using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Patients;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.UnitOfWork;
using DentalHub.Application.Factories;
using Microsoft.Extensions.Logging;
using DentalHub.Application.Specification.Comman;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.DTOs.Identity;
using Microsoft.AspNetCore.Identity;
using DentalHub.Application.Commands.Patient;
using DentalHub.Application.DTOs.Diagnoses;

namespace DentalHub.Application.Services
{
    public class PatientService : IPatientService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PatientService> _logger;
        private readonly UserManager<User> _userManager;

        public PatientService(IUnitOfWork unitOfWork, ILogger<PatientService> logger, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userManager = userManager;
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
                        NationalId = p.NationalId,
                        CreateAt = p.CreateAt,
                        Gender = p.Gender,
                        City = p.City,
                        PatientCases = p.PatientCases
                            .Select(pc => new PatientCaseSimpleDataDto
                            {
                                Id = pc.Id,
                                //     Name = pc.CaseType.Name,
                                Status = pc.Status,
                                CreateAt = pc.CreateAt,
                                UniversityId = pc.UniversityId,
                                Diagnoses = pc.Diagnosiss.Select(d => new DiagnosisSimpleDto
                                {
                                    Id = d.Id,
                                    CaseTypeName = d.CaseType.Name
                                }).ToList()
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
                        NationalId = p.NationalId,
                        CreateAt = p.CreateAt,
                        Gender = p.Gender,
                        City = p.City,
                        PatientCases = p.PatientCases
                            .Select(pc => new PatientCaseSimpleDataDto
                            {
                                Id = pc.Id,
                                //  Name = pc.CaseType.Name,
                                Status = pc.Status,
                                CreateAt = pc.CreateAt,
                                UniversityId = pc.UniversityId,
                                Diagnoses = pc.Diagnosiss.Select(d => new DiagnosisSimpleDto
                                {
                                    Id = d.Id,
                                    CaseTypeName = d.CaseType.Name
                                }).ToList()
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
                    p => (filterPatientDto.Name == null || p.User.FullName.StartsWith(filterPatientDto.Name)) &&
                         (filterPatientDto.NationalId == null || p.NationalId == filterPatientDto.NationalId) &&
                         (filterPatientDto.PhoneNumber == null || p.Phone.StartsWith(filterPatientDto.PhoneNumber)),
                    p => new PatientDto
                    {
                        PublicId = p.Id,
                        FullName = p.User.FullName,
                        Email = p.User.Email!,
                        Phone = p.Phone,
                        Age = p.Age,
                        NationalId = p.NationalId,
                        CreateAt = p.CreateAt,
                        Gender = p.Gender,
                        City = p.City,
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
                                CreateAt = pc.CreateAt,
                                UniversityId = pc.UniversityId,
                                Diagnoses = pc.Diagnosiss.Select(d => new DiagnosisSimpleDto
                                {
                                    Id = d.Id,
                                    CaseTypeName = d.CaseType.Name
                                }).ToList()
                            })
                            .ToList()
                    }
                );

                spec.AddInclude(p => p.User);
                spec.AddInclude(p => p.PatientCases);
                // CaseType is commented out on PatientCase entity - do NOT include it
                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderByDescending(p => p.CreateAt);

                // CountAsync needs a plain BaseSpecification without paging
                var countSpec = new BaseSpecification<Patient>(
                    p => (filterPatientDto.Name == null || p.User.FullName.Contains(filterPatientDto.Name)) &&
                         (filterPatientDto.NationalId == null || p.NationalId == filterPatientDto.NationalId) &&
                         (filterPatientDto.PhoneNumber == null || p.Phone.Contains(filterPatientDto.PhoneNumber)));

                var patientsList = await _unitOfWork.Patients.GetAllAsync(spec);
                var totalCount = await _unitOfWork.Patients.CountAsync(countSpec);

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

                if (!string.IsNullOrWhiteSpace(dto.NationalId))
                    patient.NationalId = dto.NationalId;

                if (dto.BirthDate.HasValue)
                {
                    var today = DateTime.Today;
                    var age = today.Year - dto.BirthDate.Value.Year;
                    if (dto.BirthDate.Value.Date > today.AddYears(-age))
                        age--;
                    patient.Age = age;
                }
                else if (dto.Age.HasValue)
                {
                    patient.Age = dto.Age.Value;
                }
                
                if (dto.Gender.HasValue)
                    patient.Gender = dto.Gender.Value;
                
                if (dto.City.HasValue)
                    patient.City = dto.City.Value;

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
                    Gender = patient.Gender,
                    City = patient.City,
                    CreateAt = patient.CreateAt,

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating patient: {PublicId}", dto.PublicId);
                return Result<PatientDto>.Failure("Error updating patient");
            }
        }

        public async Task<Result<Guid>> CreatePatientAsync(CreatePatientCommand command)
        {
            try
            {
                var existingPatientSpec = new BaseSpecification<Patient>(p => p.NationalId == command.NationalId);
                if (await _unitOfWork.Patients.AnyAsync(existingPatientSpec))
                {
                    return Result<Guid>.Failure("A patient with this National ID already exists", 400);
                }

                var today = DateTime.Today;
                var age = today.Year - command.BirthDate.Year;
                if (command.BirthDate.Date > today.AddYears(-age))
                    age--;

                await _unitOfWork.BeginTransactionAsync();

                var user = new User
                {
                    UserName = command.PhoneNumber,
                    Email = command.NationalId + "@dentalhub.com",
                    FullName = command.FullName,
                    PhoneNumber = command.PhoneNumber,
                    PhoneNumberConfirmed = true,
                };

                var result = await _userManager.CreateAsync(user, command.Password);
                if (!result.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    var errors = result.Errors.Select(e => e.Description).ToList();
                    return Result<Guid>.Failure(errors);
                }

                var roleResult = await _userManager.AddToRoleAsync(user, "Patient");
                if (!roleResult.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    var errors = roleResult.Errors.Select(e => e.Description).ToList();
                    return Result<Guid>.Failure(errors);
                }

                var patient = new Patient(user.Id)
                {
                    Age = age,
                    Phone = command.PhoneNumber,
                    NationalId = command.NationalId,
                    CreateAt = DateTime.UtcNow,
                    Gender = command.Gender,
                    City = command.City
                };

                await _unitOfWork.Patients.AddAsync(patient);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                _logger.LogInformation("Patient registered successfully: {Phone}", command.PhoneNumber);

                return Result<Guid>.Success(user.Id, "Registration successful");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error creating patient");
                return Result<Guid>.Failure("Error creating patient");
            }
        }

        public async Task<Result> HandleBeforeDeleteAsync(Guid id)
        {

            try
            {
                _logger.LogInformation("Attempting to delete patient: {Id}", id);
                var patient = await _unitOfWork.Patients.GetByIdAsync(
                    new BaseSpecificationWithProjection<Patient, GetPatientDataById>(p => p.Id == id, p =>
                    new GetPatientDataById
                    {
                        Id = p.Id,
                        HasProgressCases = p.PatientCases.Any(pc => pc.Status == CaseStatus.InProgress)
                    }));

                if (patient == null)
                    return Result.Failure("Patient not found");
                if (patient.HasProgressCases)
                    return Result.Failure("Cannot delete patient with in-progress cases");

                var patientEntity = await _unitOfWork.Patients.GetByIdAsync(new BaseSpecification<Patient>(p => p.Id == id));
                if (patientEntity != null)
                {
                    patientEntity.DeleteAt = DateTime.UtcNow;
                    _unitOfWork.Patients.Update(patientEntity);
                }

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
