using DentalHub.Application.Commands.Patient;
<<<<<<< HEAD
=======
using DentalHub.Application.Common.DentalHub.Domain.Common;
>>>>>>> 06a39604c75770df99dfba2cd9260a57c8d96007
using DentalHub.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DentalHub.Application.Common;
namespace DentalHub.Application.Services.Patient
{
 public interface IPatientService
{
    // Commands
    Task<Result<Guid>> CreateAsync(CreatePatientCommand command);
    Task<Result<bool>> UpdateAsync(UpdatePatientCommand command);
    Task<Result<bool>> DeleteAsync(Guid userId);

    // Queries
    Task<Result<PatientDto>> GetByIdAsync(Guid userId);
    Task<Result<List<PatientDto>>> GetAllAsync();
}

}

