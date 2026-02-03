using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Patients;
using DentalHub.Application.Queries.Patient;
using DentalHub.Application.Services.Patients;
using MediatR;

namespace DentalHub.Application.Handlers.Patient
{
    public class GetAllPatientsQueryHandler : IRequestHandler<GetAllPatientsQuery, Result<List<PatientDto>>>
    {
        private readonly IPatientService _service;

        public GetAllPatientsQueryHandler(IPatientService service)
        {
            _service = service;
        }

        public async Task<Result<List<PatientDto>>> Handle(GetAllPatientsQuery request, CancellationToken ct)
        {
            return await _service.GetAllPatientsAsync();
        }
    }
}
