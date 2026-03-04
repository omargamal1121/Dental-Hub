using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Patients;

namespace DentalHub.Application.Queries.Patient
{
    public record GetPatientByIdQuery(Guid PublicId) : IRequest<Result<PatientDto>>;
}
