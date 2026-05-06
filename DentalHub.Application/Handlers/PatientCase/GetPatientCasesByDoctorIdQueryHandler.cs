using System.Threading;
using System.Threading.Tasks;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Queries.PatientCase;
using DentalHub.Application.Services.Cases;
using MediatR;

namespace DentalHub.Application.Handlers.PatientCaseHandler
{
    public class GetPatientCasesByDoctorIdQueryHandler : IRequestHandler<GetPatientCasesByDoctorIdQuery, Result<PagedResult<PatientCaseDto>>>
    {
        private readonly IPatientCaseService _service;

        public GetPatientCasesByDoctorIdQueryHandler(IPatientCaseService service)
        {
            _service = service;
        }

        public async Task<Result<PagedResult<PatientCaseDto>>> Handle(GetPatientCasesByDoctorIdQuery request, CancellationToken ct)
        {
            return await _service.GetCasesByDoctorIdAsync(request.DoctorId, request.Status, request.Page, request.PageSize);
        }
    }
}
