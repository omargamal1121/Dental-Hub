using DentalHub.Application.Common;
using DentalHub.Application.DTOs;
using DentalHub.Application.Queries.Patient;
using DentalHub.Application.Services.Patient;
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

        public Task<Result<PatientDto>> Handle(GetPatientByIdQuery request, CancellationToken ct)
            => _service.GetByIdAsync(request.UserId);
    }
}
