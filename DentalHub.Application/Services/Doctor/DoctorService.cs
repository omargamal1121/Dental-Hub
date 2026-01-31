using DentalHub.Application.Common;

namespace DentalHub.Application.Services.Doctor
{
   public class DoctorService : IDoctorService
{
    private readonly IDoctorRepository _doctorRepo;

    public DoctorService(IDoctorRepository doctorRepo)
    {
        _doctorRepo = doctorRepo;
    }

    public async Task<Result<Guid>> CreateAsync(CreateDoctorCommand command)
    {
        var doctor = new Doctor
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            UserId = command.UserId,
            Specialty = command.Specialty,
            UniversityId = command.UniversityId
        };

        await _doctorRepo.AddAsync(doctor);
        return Result<Guid>.Success(doctor.Id);
    }

    public async Task<Result> UpdateAsync(UpdateDoctorCommand command)
    {
        var doctor = await _doctorRepo.GetByIdAsync(command.Id);
        if (doctor is null)
            return Result.Failure("Doctor not found");

        doctor.Name = command.Name;
        doctor.Specialty = command.Specialty;
        doctor.UniversityId = command.UniversityId;

        await _doctorRepo.UpdateAsync(doctor);
        return Result.Success();
    }

    public async Task<Result> DeleteAsync(Guid id)
    {
        var doctor = await _doctorRepo.GetByIdAsync(id);
        if (doctor is null)
            return Result.Failure("Doctor not found");

        await _doctorRepo.DeleteAsync(doctor);
        return Result.Success();
    }

    public async Task<Result<DoctorDto>> GetByIdAsync(Guid id)
    {
        var doctor = await _doctorRepo.GetByIdAsync(id);
        if (doctor is null)
            return Result<DoctorDto>.Failure("Doctor not found");

        return Result<DoctorDto>.Success(new DoctorDto(doctor));
    }

    public async Task<Result<List<DoctorDto>>> GetAllAsync()
    {
        var doctors = await _doctorRepo.GetAllAsync();
        return Result<List<DoctorDto>>.Success(
            doctors.Select(d => new DoctorDto(d)).ToList());
    }
}

}

