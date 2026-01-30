using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Services.PatientCase
{
   public interface IPatientCaseService
{
    // Commands
    Task<Result<Guid>> CreateAsync(CreatePatientCaseCommand command);
    Task<Result> UpdateAsync(UpdatePatientCaseCommand command);
    Task<Result> DeleteAsync(Guid id);

    // Queries
    Task<Result<PatientCaseDto>> GetByIdAsync(Guid id);
    Task<Result<List<PatientCaseDto>>> GetByPatientIdAsync(Guid patientId);
}

}

