using DentalHub.Application.Common;
using DentalHub.Application.Commands.Patient;
using DentalHub.Application.DTOs;
using DentalHub.Infrastructure.UnitOfWork;
using DentalHub.Infrastructure.Specification;
using DentalHub.Domain.Entities;

namespace DentalHub.Application.Services.PatientServcie
{
  public class PatientService : IPatientService
{
    private readonly IUnitOfWork  _unitOfWork;

    public PatientService(IUnitOfWork unitOfWork )
    {
       _unitOfWork = unitOfWork;
		}

    public async Task<Result<Guid>> CreateAsync(CreatePatientCommand command)
    {
        var patient = new Patient
        {
            UserId = command.UserId,
            Age = command.Age,
            Phone = command.Phone
        };

        await _unitOfWork.Patients.AddAsync(patient);
        return Result<Guid>.Success(patient.UserId);
    }

    public async Task<Result<bool>> UpdateAsync(UpdatePatientCommand command)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(command.UserId);
        if (patient is null)
            return Result<bool>.Failure("Patient not found");

        patient.Age = command.Age;
        patient.Phone = command.Phone;

         _unitOfWork.Patients.Update(patient);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid userId)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(userId);
        if (patient is null)
            return Result<bool>.Failure("Patient not found");

         _unitOfWork.Patients.Remove(patient);
        return Result<bool>.Success(true);
    }

    public async Task<Result<PatientDto>> GetByIdAsync(Guid userId)
    {
        var patient = await _unitOfWork.Patients.GetByIdAsync(userId);
        if (patient is null)
            return Result<PatientDto>.Failure("Patient not found");

        return Result<PatientDto>.Success(new PatientDto(patient));
    }

    public async Task<Result<List<PatientDto>>> GetAllAsync()
    {
        var patients = await _unitOfWork.Patients.GetAllAsync(new BaseSpecification<Patient> { });
        return Result<List<PatientDto>>.Success(
            patients.Select(p => new PatientDto(p)).ToList());
    }
}

}

