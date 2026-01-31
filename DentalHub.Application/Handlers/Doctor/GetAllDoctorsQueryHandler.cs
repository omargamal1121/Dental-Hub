using DentalHub.Application.Common;
using DentalHub.Application.Common.DentalHub.Domain.Common;

using DentalHub.Application.DTOs;
using DentalHub.Application.Queries.Doctor;
using DentalHub.Application.Services.Doctor;
using MediatR;

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
