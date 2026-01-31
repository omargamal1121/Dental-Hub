using DentalHub.Application.Common;
using DentalHub.Application.DTOs;
using MediatR;

namespace DentalHub.Application.Queries.Patient
{
   public record GetAllPatientsQuery()
    : IRequest<Result<List<PatientDto>>>;
}

