using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Patients;
using DentalHub.Application.Queries.Patient;
using DentalHub.Application.Services.Patients;
using MediatR;

namespace DentalHub.Application.Handlers.Patient
{
    public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, Result<PatientDto>>
    {
        private readonly IPatientService _service;

        public GetPatientByIdQueryHandler(IPatientService service)
        {
            _service = service;
        }

        public async Task<Result<PatientDto>> Handle(GetPatientByIdQuery request, CancellationToken ct)
        {
            return await _service.GetPatientByIdAsync(request.UserId);
        }
    }
}
