using DentalHub.Application.Commands.Patient;
using DentalHub.Application.Common;
using DentalHub.Application.Services.Identity;
using DentalHub.Application.DTOs.Identity;
using MediatR;

namespace DentalHub.Application.Handlers.Patient
{
    public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, Result<Guid>>
    {
        private readonly IUserManagementService _userManagementService;

        public CreatePatientCommandHandler(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        public async Task<Result<Guid>> Handle(CreatePatientCommand request, CancellationToken ct)
        {
            // Use real user data from the request
            var registerDto = new RegisterPatientDto
            {
                FullName = request.FullName,
                Email = request.Email,         
                Password = request.Password, 
                Phone = request.Phone,
                Age = request.Age
            };

            var result = await _userManagementService.RegisterPatientAsync(registerDto);

            if (!result.IsSuccess)
            {
                return Result<Guid>.Failure(result.Message ?? "Patient creation failed");
            }

            return Result<Guid>.Success(result.Data.UserId, "Patient created successfully");
        }
    }
}
