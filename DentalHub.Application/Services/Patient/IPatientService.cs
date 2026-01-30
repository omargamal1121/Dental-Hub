using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Services.Patient
{
 public interface IPatientService
{
    // Commands
    Task<Result<Guid>> CreateAsync(CreatePatientCommand command);
    Task<Result> UpdateAsync(UpdatePatientCommand command);
    Task<Result> DeleteAsync(Guid userId);

    // Queries
    Task<Result<PatientDto>> GetByIdAsync(Guid userId);
    Task<Result<List<PatientDto>>> GetAllAsync();
}

}

