using DentalHub.Application.Common;
using DentalHub.Application.DTOs;
using MediatR;

namespace DentalHub.Application.Queries.PatientCase
{
  public record GetPatientCasesByPatientIdQuery(Guid PatientId)
    : IRequest<Result<List<PatientCaseDto>>>;
}

