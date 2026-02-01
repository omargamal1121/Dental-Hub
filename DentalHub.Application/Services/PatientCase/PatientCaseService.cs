using DentalHub.Application.Commands.PatientCase;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.Specification;
using DentalHub.Infrastructure.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Services.PatientCaseService
{
 public class PatientCaseService : IPatientCaseService
{
    private readonly IUnitOfWork    _unitOfWork;

    public PatientCaseService(IUnitOfWork  unitOfWork)
    {
        _unitOfWork =  unitOfWork;
    }

    public async Task<Result<Guid>> CreateAsync(CreatePatientCaseCommand command)
    {
        var patientCase = new PatientCase
        {
            Id = Guid.NewGuid(),
            PatientId = command.PatientId,
            TreatmentType = command.TreatmentType,
            Status = CaseStatus.Pending
        };

        await _unitOfWork.PatientCases.AddAsync(patientCase);
        return Result<Guid>.Success(patientCase.Id);
    }

    public async Task<Result<bool>> UpdateAsync(UpdatePatientCaseCommand command)
    {
        var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(command.Id);
        if (patientCase is null)
            return Result<bool>.Failure("Patient case not found");

        patientCase.TreatmentType = command.TreatmentType;
        patientCase.Status = command.Status;

         _unitOfWork.PatientCases.Update(patientCase);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(id);
        if (patientCase is null)
            return Result<bool>.Failure("Patient case not found");

        _unitOfWork.PatientCases.Update(patientCase);
        return Result<bool>.Success(true);
    }

    public async Task<Result<PatientCaseDto>> GetByIdAsync(Guid id)
    {
        var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(id);
        if (patientCase is null)
            return Result<PatientCaseDto>.Failure("Patient case not found");

        return Result<PatientCaseDto>.Success(new PatientCaseDto(patientCase));
    }

    //public async Task<Result<List<PatientCaseDto>>> GetByPatientIdAsync(Guid patientId)
    //{
    //    var cases = await _unitOfWork.PatientCases.GetByIdAsync(new BaseSpecification<PatientCase> { });
    //    return Result<List<PatientCaseDto>>.Success(
    //        cases.Select(cases);
    //}
}

}

