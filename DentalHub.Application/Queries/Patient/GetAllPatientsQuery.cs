using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Patients;
using DentalHub.Application.Services;

namespace DentalHub.Application.Queries.Patient
{
    public record GetAllPatientsQuery(FilterPatientDto FilterPatientDto, int PageNumber = 1, int PageSize = 10) : IRequest<Result<PagedResult<PatientDto>>>;
}
