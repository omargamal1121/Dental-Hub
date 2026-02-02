using DentalHub.Application.Common;
using MediatR;
using DentalHub.Application.DTOs.Cases;

namespace DentalHub.Application.Queries.PatientCase
{
    public record GetPatientCaseByIdQuery(Guid Id)
    : IRequest<Result<PatientCaseDto>>;
}
