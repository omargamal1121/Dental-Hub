using DentalHub.Application.Commands.Patient;
using DentalHub.Application.Common;
using DentalHub.Application.Services.Identity;
using DentalHub.Application.DTOs.Identity;
using MediatR;

namespace DentalHub.Application.Handlers.Patient
{
    public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, Result<string>>
    {
        private readonly IUserManagementService _userManagementService;

        public CreatePatientCommandHandler(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        public async Task<Result<string>> Handle(CreatePatientCommand request, CancellationToken ct)
        {
            var registerDto = new RegisterPatientDto
            {
                FullName = request.FullName,
                City= request.City,
              
                Password = request.Password,
                Phone = request.PhoneNumber,
                NationalId = request.NationalId,
                BirthDate = request.BirthDate,
                Gender = request.Gender
            };

            var result = await _userManagementService.RegisterPatientAsync(registerDto);

            if (!result.IsSuccess)
            {
                return Result<string>.Failure(result.Errors ?? new List<string> { result.Message ?? "Patient creation failed" }, result.Status);
            }

            return Result<string>.Success(result.Data.PublicId, result.Message, result.Status);
        }
    }
}
