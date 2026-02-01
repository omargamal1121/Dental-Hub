using DentalHub.Application.Commands.Doctor;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.Specification;
using DentalHub.Infrastructure.UnitOfWork;

namespace DentalHub.Application.Services.DoctorService
{
   public class DoctorService : IDoctorService
{
    private readonly IUnitOfWork _unitOfWork;

    public DoctorService(IUnitOfWork unitOfWork )
    {
        _unitOfWork = unitOfWork;
		}

    public async Task<Result<Guid>> CreateAsync(CreateDoctorCommand command)
    {
        var doctor = new Doctor
        {
          
            Name = command.Name,
            UserId = command.UserId,
            Specialty = command.Specialty,
            UniversityId = command.UniversityId
        };

        await _unitOfWork.Doctors.AddAsync(doctor);
        return Result<Guid>.Success(new Guid());
    }

    public async Task<Result<bool>> UpdateAsync(UpdateDoctorCommand command)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(command.Id);
        if (doctor is null)
            return Result<bool>.Failure("Doctor not found");

        doctor.Name = command.Name;
        doctor.Specialty = command.Specialty;
        doctor.UniversityId = command.UniversityId;

         _unitOfWork.Doctors.Update(doctor);
        return Result<bool>.Success(true);
    }

    public async Task<Result<bool>> DeleteAsync(Guid id)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);
        if (doctor is null)
            return Result<bool>.Failure("Doctor not found");

         _unitOfWork.Doctors.Remove(doctor);
        return Result<bool>.Success(true);
    }

    public async Task<Result<DoctorDto>> GetByIdAsync(Guid id)
    {
        var doctor = await _unitOfWork.Doctors.GetByIdAsync(id);
        if (doctor is null)
            return Result<DoctorDto>.Failure("Doctor not found");

        return Result<DoctorDto>.Success(new DoctorDto(doctor));
    }

    public async Task<Result<List<DoctorDto>>> GetAllAsync()
    {
        var doctors = await _unitOfWork.Doctors.GetAllAsync(new BaseSpecification<Doctor> { });
        return Result<List<DoctorDto>>.Success(
            doctors.Select(d => new DoctorDto(d)).ToList());
    }
}

}

