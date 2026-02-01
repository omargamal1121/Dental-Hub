using DentalHub.Application.Common;
<<<<<<< HEAD
=======
using DentalHub.Application.Common.DentalHub.Domain.Common;
>>>>>>> 06a39604c75770df99dfba2cd9260a57c8d96007
using DentalHub.Application.DTOs;
using DentalHub.Application.Queries.Doctor;
using DentalHub.Application.Services.Doctor;
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

        public Task<Result<DoctorDto>> Handle(GetDoctorByIdQuery request, CancellationToken ct)
            => _service.GetByIdAsync(request.Id);
    }
}
