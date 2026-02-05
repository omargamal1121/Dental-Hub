using DentalHub.Application.Commands.Doctor;
using DentalHub.Application.Common;
using DentalHub.Application.Services.Identity;
using DentalHub.Application.DTOs.Identity;
using MediatR;

namespace DentalHub.Application.Handlers.Doctor
{
    public class CreateDoctorCommandHandler : IRequestHandler<CreateDoctorCommand, Result<Guid>>
    {
        private readonly IUserManagementService _userManagementService;

        public CreateDoctorCommandHandler(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        public async Task<Result<Guid>> Handle(CreateDoctorCommand request, CancellationToken ct)
        {
            var registerDto = new RegisterDoctorDto
            {
                FullName = request.Name,
                Email = request.Email,
                Password = request.Password,
                Name = request.Name,
                Specialty = request.Specialty,
                UniversityId = request.UniversityId
            };

            var result = await _userManagementService.RegisterDoctorAsync(registerDto);

            if (!result.IsSuccess)
            {
                return Result<Guid>.Failure(result.Errors ?? new List<string> { result.Message ?? "Creation failed" }, result.Status);
            }

            return Result<Guid>.Success(result.Data.UserId, result.Message, result.Status);
        }
    }
}
