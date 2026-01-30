using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Services.PatientCase
{
 public class PatientCaseService : IPatientCaseService
{
    private readonly IPatientCaseRepository _caseRepo;

    public PatientCaseService(IPatientCaseRepository caseRepo)
    {
        _caseRepo = caseRepo;
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

        await _caseRepo.AddAsync(patientCase);
        return Result<Guid>.Success(patientCase.Id);
    }

    public async Task<Result> UpdateAsync(UpdatePatientCaseCommand command)
    {
        var patientCase = await _caseRepo.GetByIdAsync(command.Id);
        if (patientCase is null)
            return Result.Failure("Patient case not found");

        patientCase.TreatmentType = command.TreatmentType;
        patientCase.Status = command.Status;

        await _caseRepo.UpdateAsync(patientCase);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var patientCase = await _caseRepo.GetByIdAsync(id);
        if (patientCase is null)
            return Result.Failure("Patient case not found");

        await _caseRepo.DeleteAsync(patientCase);
        return Result.Success();
    }

    public async Task<Result<PatientCaseDto>> GetByIdAsync(Guid id)
    {
        var patientCase = await _caseRepo.GetByIdAsync(id);
        if (patientCase is null)
            return Result<PatientCaseDto>.Failure("Patient case not found");

        return Result<PatientCaseDto>.Success(new PatientCaseDto(patientCase));
    }

    public async Task<Result<List<PatientCaseDto>>> GetByPatientIdAsync(Guid patientId)
    {
        var cases = await _caseRepo.GetByPatientIdAsync(patientId);
        return Result<List<PatientCaseDto>>.Success(
            cases.Select(c => new PatientCaseDto(c)).ToList());
    }
}

}

