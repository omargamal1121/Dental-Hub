using DentalHub.Application.Commands.v2.PatientCase;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.DTOs.Diagnoses;
using DentalHub.Application.Interfaces;
using DentalHub.Application.Services.Cases;
using DentalHub.Application.Services.DiagnosesServices;
using DentalHub.Application.Specification.Comman;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.UnitOfWork;
using MediatR;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DentalHub.Application.Handlers.v2.PatientCaseHandler
{
    public class CreatePatientCaseV2CommandHandler : IRequestHandler<CreatePatientCaseV2Command, Result<Guid>>
    {
        private readonly IPatientCaseService _caseService;
        private readonly IDiagnosisService _diagnosisService;
        private readonly IUnitOfWork _unitOfWork;

        public CreatePatientCaseV2CommandHandler(
            IPatientCaseService caseService,
            IDiagnosisService diagnosisService,
            IUnitOfWork unitOfWork)
        {
            _caseService = caseService;
            _diagnosisService = diagnosisService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreatePatientCaseV2Command request, CancellationToken ct)
        {
            // 1. Find patient by NationalId
            var patientSpec = new BaseSpecification<Patient>(p => p.NationalId == request.NationalId);
            var patient = await _unitOfWork.Patients.GetByIdAsync(patientSpec);

            if (patient == null)
            {
                return Result<Guid>.Failure("Patient with the provided National ID was not found.", 404);
            }

            // 2. Create the case
            var createCaseDto = new CreateCaseDto
            {
                PatientId = patient.Id,
             
                Description = request.Description,
              
                IsPublic = request.IsPublic,
                UniversityId = request.UniversityId,
                Images = request.Images,
                CreatedById = request.CreatedById,
                CreatedByRole = request.CreatedByRole
            };

            var caseResult = await _caseService.CreateCaseAsync(createCaseDto);

            if (!caseResult.IsSuccess||caseResult.Data == null)
            {
                return Result<Guid>.Failure(caseResult.Errors ?? new List<string> { caseResult.Message ?? "Failed to create case" }, caseResult.Status);
            }

            var caseId = caseResult.Data.Id;

    
            if (request.InitialDiagnosis != null)
            {
                var createDiagnosisDto = new CreateDiagnosisDto
                {
                    PatientCaseId = caseId,
                    Stage = request.InitialDiagnosis.Stage,
                    CaseTypeId = request.InitialDiagnosis.CaseTypeId, 
                    Notes = request.InitialDiagnosis.Notes,
                    TeethNumbers = request.InitialDiagnosis.TeethNumbers
                };

                var diagnosisResult = await _diagnosisService.CreateDiagnosisAsync(createDiagnosisDto, request.CreatedById, request.CreatedByRole);

                if (!diagnosisResult.IsSuccess)
                {
                    return Result<Guid>.Success(caseId, "Case created, but initial diagnosis failed: " + diagnosisResult.Message, 201);
                }
            }

            return Result<Guid>.Success(caseId, "Case created successfully" + (request.InitialDiagnosis != null ? " with initial diagnosis" : ""), 201);
        }
    }
}
