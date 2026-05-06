using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Diagnoses;
using DentalHub.Application.Factories;
using DentalHub.Application.Interfaces;
using DentalHub.Application.Queries.v2.Diagnoses;
using DentalHub.Application.Specification.Comman;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DentalHub.Application.Handlers.v2.Diagnoses
{
    public class GetDiagnosesByNationalIdQueryHandler 
        : IRequestHandler<GetDiagnosesByNationalIdQuery, Result<PagedResult<DiagnosisDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetDiagnosesByNationalIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResult<DiagnosisDto>>> Handle(
            GetDiagnosesByNationalIdQuery request, CancellationToken ct)
        {
            // 1. Find the patient first to get their Id
            var patientSpec = new BaseSpecification<Patient>(p => p.NationalId == request.NationalId);
            var patient = await _unitOfWork.Patients.GetByIdAsync(patientSpec);

            if (patient == null)
            {
                return Result<PagedResult<DiagnosisDto>>.Failure("Patient with the provided National ID was not found.", 404);
            }

            // 2. Query diagnoses for this patient
            // We need to reach PatientId through PatientCase
            var diagnosisSpec = new BaseSpecificationWithProjection<Diagnosis, DiagnosisDto>(
                criteria: d => d.PatientCase.PatientId == patient.Id,
                projection: d => new DiagnosisDto
                {
                    Id = d.Id,
                    PatientCaseId = d.PatientCaseId,
                    CaseTypeId = d.CaseTypeId,
                    CaseTypeName = d.CaseType.Name,
                    Stage = d.Stage,
                    Notes = d.Notes,
                    TeethNumbers = d.TeethNumbers,
                    IsAccepted = d.IsAccepted,
                   
                }
            );

            diagnosisSpec.ApplyPaging(request.Page, request.PageSize);
            diagnosisSpec.ApplyOrderByDescending(d => d.CreateAt);

            var countSpec = new BaseSpecification<Diagnosis>(d => d.PatientCase.PatientId == patient.Id);

            var diagnosesList = await _unitOfWork.Diagnoses.GetAllAsync(diagnosisSpec);
            var totalCount = await _unitOfWork.Diagnoses.CountAsync(countSpec);

            var pagedResult = PaginationFactory<DiagnosisDto>.Create(
                count: totalCount, page: request.Page, pageSize: request.PageSize, data: diagnosesList);

            return Result<PagedResult<DiagnosisDto>>.Success(pagedResult);
        }
    }
}
