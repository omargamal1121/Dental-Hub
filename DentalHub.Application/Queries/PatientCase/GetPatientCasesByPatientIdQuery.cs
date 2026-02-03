using DentalHub.Application.Common;
using MediatR;
using DentalHub.Application.DTOs.Cases;

namespace DentalHub.Application.Queries.PatientCase
{
  public record GetPatientCasesByPatientIdQuery(Guid PatientId)
    : IRequest<Result<List<PatientCaseDto>>>;
}
