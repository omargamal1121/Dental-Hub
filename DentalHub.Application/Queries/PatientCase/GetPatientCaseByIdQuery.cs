using DentalHub.Application.Common;
using DentalHub.Application.DTOs;
using MediatR;

namespace DentalHub.Application.Queries.PatientCase
{
    public record GetPatientCaseByIdQuery(Guid Id)
    : IRequest<Result<PatientCaseDto>>;
}

