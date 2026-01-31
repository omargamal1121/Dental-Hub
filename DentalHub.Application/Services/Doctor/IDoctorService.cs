using DentalHub.Application.Commands.Doctor;
using DentalHub.Application.Common.DentalHub.Domain.Common;
using DentalHub.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Services.Doctor
{
   public interface IDoctorService
{
    // Commands
    Task<Result<Guid>> CreateAsync(CreateDoctorCommand command);
    Task<Result> UpdateAsync(UpdateDoctorCommand command);
    Task<Result> DeleteAsync(Guid id);

    // Queries
    Task<Result<DoctorDto>> GetByIdAsync(Guid id);
    Task<Result<List<DoctorDto>>> GetAllAsync();
}

}

