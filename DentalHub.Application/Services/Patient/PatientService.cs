using DentalHub.Application.Common;
using DentalHub.Domain.Entities;
using DentalHub.Application.Commands.Patient;
using DentalHub.Application.DTOs;

namespace DentalHub.Application.Services.Patient
{
  public class PatientService : IPatientService
{
    private readonly IPatientRepository _patientRepo;

    public PatientService(IPatientRepository patientRepo)
    {
        _patientRepo = patientRepo;
    }

    public async Task<Result<Guid>> CreateAsync(CreatePatientCommand command)
    {
        var patient = new Patient
        {
            UserId = command.UserId,
            Age = command.Age,
            Phone = command.Phone
        };

        await _patientRepo.AddAsync(patient);
        return Result<Guid>.Success(patient.UserId);
    }

    public async Task<Result> UpdateAsync(UpdatePatientCommand command)
    {
        var patient = await _patientRepo.GetByIdAsync(command.UserId);
        if (patient is null)
            return Result.Failure("Patient not found");

        patient.Age = command.Age;
        patient.Phone = command.Phone;

        await _patientRepo.UpdateAsync(patient);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid userId)
    {
        var patient = await _patientRepo.GetByIdAsync(userId);
        if (patient is null)
            return Result.Failure("Patient not found");

        await _patientRepo.DeleteAsync(patient);
        return Result.Success();
    }

    public async Task<Result<PatientDto>> GetByIdAsync(Guid userId)
    {
        var patient = await _patientRepo.GetByIdAsync(userId);
        if (patient is null)
            return Result<PatientDto>.Failure("Patient not found");

        return Result<PatientDto>.Success(new PatientDto(patient));
    }

    public async Task<Result<List<PatientDto>>> GetAllAsync()
    {
        var patients = await _patientRepo.GetAllAsync();
        return Result<List<PatientDto>>.Success(
            patients.Select(p => new PatientDto(p)).ToList());
    }
}

}

