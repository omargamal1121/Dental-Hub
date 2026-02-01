using DentalHub.Application.Commands.Doctor;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs;

namespace DentalHub.Application.Services.DoctorService
{
   public interface IDoctorService
{
    // Commands
    Task<Result<Guid>> CreateAsync(CreateDoctorCommand command);
    Task<Result<bool>> UpdateAsync(UpdateDoctorCommand command);
    Task<Result<bool>> DeleteAsync(Guid id);

    // Queries
    Task<Result<DoctorDto>> GetByIdAsync(Guid id);
    Task<Result<List<DoctorDto>>> GetAllAsync();
}

}

