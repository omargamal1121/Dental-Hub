

using DentalHub.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DentalHub.Application.Common;
using DentalHub.Application.Commands.Patient;
namespace DentalHub.Application.Services.PatientServcie
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

