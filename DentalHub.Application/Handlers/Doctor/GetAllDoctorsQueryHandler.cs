using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Doctors;
using DentalHub.Application.Queries.Doctor;
using DentalHub.Application.Services;
using DentalHub.Application.Services.Doctors;
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

        public async Task<Result<List<DoctorDto>>> Handle(GetAllDoctorsQuery request, CancellationToken ct)
        {
            //Service_GetAllDoctorsAsync_GetAllAsync
            return await _service.GetAllDoctorsAsync();
        }
    }
}
