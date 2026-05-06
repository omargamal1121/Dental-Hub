using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Doctors;
using DentalHub.Application.Queries.Doctor;
using DentalHub.Application.Services.Identity;
using MediatR;

namespace DentalHub.Application.Handlers.Doctor
{
    public class GetClinicalDoctorsQueryHandler : IRequestHandler<GetClinicalDoctorsQuery, Result<List<DoctorlistDto>>>
    {
        private readonly IUserManagementService _userService;

        public GetClinicalDoctorsQueryHandler(IUserManagementService userService)
        {
            _userService = userService;
        }

        public async Task<Result<List<DoctorlistDto>>> Handle(GetClinicalDoctorsQuery request, CancellationToken ct)
        {
            return await _userService.GetClinicalDoctorsAsync();
        }
    }
}
