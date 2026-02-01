using DentalHub.Application.Commands.PatientCase;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs;

namespace DentalHub.Application.Services.PatientCaseService
{
   public interface IPatientCaseService
{
    // Commands
    Task<Result<Guid>> CreateAsync(CreatePatientCaseCommand command);
    Task<Result<bool>> UpdateAsync(UpdatePatientCaseCommand command);
    Task<Result<bool>> DeleteAsync(Guid id);

    // Queries
    Task<Result<PatientCaseDto>> GetByIdAsync(Guid id);
    //Task<Result<List<PatientCaseDto>>> GetByPatientIdAsync(Guid patientId);
}

}

