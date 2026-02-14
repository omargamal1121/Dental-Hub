using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Doctors;
using DentalHub.Application.Queries.Doctor;
using DentalHub.Application.Services.Doctors;
using MediatR;

namespace DentalHub.Application.Handlers.Doctor
{
    public class GetAllDoctorsQueryHandler : IRequestHandler<GetAllDoctorsQuery, Result<PagedResult<DoctorlistDto>>>
    {
        private readonly IDoctorService _service;

        public GetAllDoctorsQueryHandler(IDoctorService service)
        {
            _service = service;
        }

        public async Task<Result<PagedResult<DoctorlistDto>>> Handle(GetAllDoctorsQuery request, CancellationToken ct)
        {
            return await _service.GetAllDoctorsAsync(request.Page, request.PageSize, request.Name, request.Spec);
        }
    }
}
