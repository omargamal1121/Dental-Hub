using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Patients;

namespace DentalHub.Application.Queries.Patient
{
    public record GetAllPatientsQuery(int PageNumber = 1, int PageSize = 10) : IRequest<Result<List<PatientDto>>>;
}
