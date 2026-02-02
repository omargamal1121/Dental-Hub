using DentalHub.Application.Common;
using MediatR;
using DentalHub.Application.DTOs.Patients;

namespace DentalHub.Application.Queries.Patient
{
   public record GetAllPatientsQuery()
    : IRequest<Result<List<PatientDto>>>;
}
