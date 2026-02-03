using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Doctors;
using DentalHub.Application.Queries.Doctor;
using DentalHub.Application.Services;
using DentalHub.Application.Services.Doctors;
using MediatR;

namespace DentalHub.Application.Handlers.Doctor
{
    public class GetDoctorByIdQueryHandler : IRequestHandler<GetDoctorByIdQuery, Result<DoctorDto>>
    {
        private readonly IDoctorService _service;

        public GetDoctorByIdQueryHandler(IDoctorService service)
        {
            _service = service;
        }

        public async Task<Result<DoctorDto>> Handle(GetDoctorByIdQuery request, CancellationToken ct)
        {
            // Service _GetDoctorByIdAsync _GetByIdAsync
            return await _service.GetDoctorByIdAsync(request.Id);
        }
    }
}
