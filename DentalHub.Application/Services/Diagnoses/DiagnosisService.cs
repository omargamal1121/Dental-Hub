using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Diagnoses;
using DentalHub.Application.Factories;
using DentalHub.Application.Specification.Comman;
using DentalHub.Domain.DomainExceptions;
using DentalHub.Domain.Entities;
using DentalHub.Domain.Factories;
using DentalHub.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data.Entity;

namespace DentalHub.Application.Services.DiagnosesServices
{
    public class DiagnosisService : IDiagnosisService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DiagnosisService> _logger;
        private readonly UserManager<User> _userManager;
        

		public DiagnosisService(IUnitOfWork unitOfWork, ILogger<DiagnosisService> logger, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _userManager = userManager;
        }

        public async Task<Result<DiagnosisDto>> GetDiagnosisByIdAsync(Guid id)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Diagnosis, DiagnosisDto>(
                    d => d.Id == id,
                    d => new DiagnosisDto
                    {
                        Id = d.Id,
                        PatientCaseId = d.PatientCaseId,
                        Stage = d.Stage,
                        CaseTypeId = d.CaseTypeId,
                        CaseTypeName = d.CaseType.Name,
                        Notes = d.Notes,
                        CreatedById = d.CreatedById,
                        Role = d.Role,
                        IsAccepted = d.IsAccepted,
                        TeethNumbers = d.TeethNumbers
                    }
                );

                var diagnosis = await _unitOfWork.Diagnoses.GetByIdAsync(spec);

                if (diagnosis == null)
                {
                    return Result<DiagnosisDto>.Failure("Diagnosis not found", 404);
                }

                return Result<DiagnosisDto>.Success(diagnosis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting diagnosis by ID: {Id}", id);
                return Result<DiagnosisDto>.Failure("Error retrieving diagnosis", 500);
            }
        }

        public async Task<Result<PagedResult<DiagnosisDto>>> GetDiagnosesByPatientCaseIdAsync(Guid patientCaseId, int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Diagnosis, DiagnosisDto>(
                    d => d.PatientCaseId == patientCaseId,
                    d => new DiagnosisDto
                    {
                        Id = d.Id,
                        PatientCaseId = d.PatientCaseId,
                        Stage = d.Stage,
                        CaseTypeId = d.CaseTypeId,
                        CaseTypeName = d.CaseType.Name,
                        Notes = d.Notes,
                        CreatedById = d.CreatedById,
                        Role = d.Role,
                        IsAccepted = d.IsAccepted,
                        TeethNumbers = d.TeethNumbers
                    }
                );

                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderByDescending(d => d.Stage); 

                var diagnosesList = await _unitOfWork.Diagnoses.GetAllAsync(spec);
                var totalCount = await _unitOfWork.Diagnoses.CountAsync(spec);

                var pagedResult = PaginationFactory<DiagnosisDto>.Create(
                    count: totalCount,
                    page: page,
                    pageSize: pageSize,
                    data: diagnosesList
                );

                return Result<PagedResult<DiagnosisDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting diagnoses for patient case: {PatientCaseId}", patientCaseId);
                return Result<PagedResult<DiagnosisDto>>.Failure("Error retrieving diagnoses", 500);
            }
        }

		public async Task<Result<DiagnosisDto>> CreateDiagnosisAsync(CreateDiagnosisDto dto, Guid? userId, string? userRole)
		{
			try
			{
				var patientCase = await _unitOfWork.PatientCases.AnyAsync(new BaseSpecification<PatientCase>(pc => pc.Id == dto.PatientCaseId
					&& (pc.Status == CaseStatus.UnderReview || pc.Status == CaseStatus.Pending)));
				if (!patientCase)
					return Result<DiagnosisDto>.Failure("Patient case not found", 404);

				var caseType = await _unitOfWork.CaseTypes.AnyAsync(new BaseSpecification<CaseType>(ct => ct.Id == dto.CaseTypeId));
				if (!caseType)
					return Result<DiagnosisDto>.Failure("Case type not found", 404);

				var diagnosis = DiagnosisFactory.Create(
					dto.PatientCaseId,
					dto.Stage,
					dto.CaseTypeId,
					dto.Notes,
					userId,  
					userRole ?? "AI",
					dto.TeethNumbers
				);

				await _unitOfWork.Diagnoses.AddAsync(diagnosis);
				await _unitOfWork.SaveChangesAsync();
				return await GetDiagnosisByIdAsync(diagnosis.Id);
			}
			catch (DomainException ex)
			{
				return Result<DiagnosisDto>.Failure(ex.Message, 400);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating diagnosis");
				return Result<DiagnosisDto>.Failure("Error creating diagnosis", 500);
			}
		}

		public async Task<Result<DiagnosisDto>> UpdateDiagnosisAsync(UpdateDiagnosisDto dto)
        {
            try
            {
                var diagnosis = await _unitOfWork.Diagnoses.GetByIdAsync(new BaseSpecification<Diagnosis>(d => d.Id == dto.Id));

                if (diagnosis == null)
                {
                    return Result<DiagnosisDto>.Failure("Diagnosis not found", 404);
                }

                var isCaseActive = await _unitOfWork.PatientCases.AnyAsync(new BaseSpecification<PatientCase>(pc => pc.Id == diagnosis.PatientCaseId
                    && (pc.Status == CaseStatus.UnderReview || pc.Status == CaseStatus.Pending || pc.Status == CaseStatus.InProgress)));
                
                if (!isCaseActive)
                {
                    return Result<DiagnosisDto>.Failure("Cannot modify a diagnosis for a completed or cancelled case.", 400);
                }

                if (dto.Stage.HasValue)
                {
                    diagnosis.Stage = dto.Stage.Value;
                }

                if (dto.CaseTypeId.HasValue)
                {

                    var caseType = await _unitOfWork.CaseTypes.GetByIdAsync(new BaseSpecification<CaseType>(ct => ct.Id == dto.CaseTypeId.Value));
                    if (caseType == null)
                    {
                        return Result<DiagnosisDto>.Failure("Case type not found", 404);
                    }
                    diagnosis.CaseTypeId = dto.CaseTypeId.Value;
                }

                if (dto.Notes != null)
                {
                    diagnosis.Notes = dto.Notes;
                }

                if (dto.TeethNumbers != null)
                {
                    diagnosis.TeethNumbers = dto.TeethNumbers;
                }

                _unitOfWork.Diagnoses.Update(diagnosis);
                await _unitOfWork.SaveChangesAsync();

                return await GetDiagnosisByIdAsync(diagnosis.Id);
            }
            catch (DomainException ex)
            {
                return Result<DiagnosisDto>.Failure(ex.Message, 400);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating diagnosis: {Id}", dto.Id);
                return Result<DiagnosisDto>.Failure("Error updating diagnosis", 500);
            }
        }

        public async Task<Result> DeleteDiagnosisAsync(Guid id)
        {
            try
            {
                var diagnosis = await _unitOfWork.Diagnoses.GetByIdAsync(new BaseSpecification<Diagnosis>(d => d.Id == id));

                if (diagnosis == null)
                {
                    return Result.Failure("Diagnosis not found", 404);
                }

                _unitOfWork.Diagnoses.Remove(diagnosis);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting diagnosis: {Id}", id);
                return Result.Failure("Error deleting diagnosis", 500);
            }
        }

        public async Task<Result> AcceptDiagnosisAsync(Guid id)
        {
            try
            {
                var diagnosis = await _unitOfWork.Diagnoses.GetByIdAsync(new BaseSpecification<Diagnosis>(d => d.Id == id));

                if (diagnosis == null)
                {
                    return Result.Failure("Diagnosis not found", 404);
                }

                diagnosis.IsAccepted = true;
                _unitOfWork.Diagnoses.Update(diagnosis);

                var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(new BaseSpecification<PatientCase>(pc => pc.Id == diagnosis.PatientCaseId));
                if (patientCase != null && patientCase.Status == CaseStatus.UnderReview)
                {
                    patientCase.Status = CaseStatus.InProgress;
                    _unitOfWork.PatientCases.Update(patientCase);
                }

                await _unitOfWork.SaveChangesAsync();

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error accepting diagnosis: {Id}", id);
                return Result.Failure("Error accepting diagnosis", 500);
            }
        }
    }
}
