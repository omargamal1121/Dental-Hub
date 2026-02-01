using DentalHub.Application.Common;
<<<<<<< HEAD
=======
using DentalHub.Application.Common.DentalHub.Domain.Common;

>>>>>>> 06a39604c75770df99dfba2cd9260a57c8d96007
using DentalHub.Application.DTOs;
using DentalHub.Application.Queries.Doctor;
using DentalHub.Application.Services.Doctor;
using MediatR;
<<<<<<< HEAD
using DentalHub.Application.Handlers;
=======

>>>>>>> 06a39604c75770df99dfba2cd9260a57c8d96007
namespace DentalHub.Application.Handlers.Doctor
{
    public class GetAllDoctorsQueryHandler : IRequestHandler<GetAllDoctorsQuery, Result<List<DoctorDto>>>
    {
        private readonly IDoctorService _service;

        public GetAllDoctorsQueryHandler(IDoctorService service)
        {
            _service = service;
        }

        public Task<Result<List<DoctorDto>>> Handle(GetAllDoctorsQuery request, CancellationToken ct)
            => _service.GetAllAsync();
    }
}
