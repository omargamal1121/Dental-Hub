using DentalHub.Application.Commands.Students;
using DentalHub.Application.Common;
using DentalHub.Application.Services.Identity;
using DentalHub.Application.DTOs.Identity;
using MediatR;

namespace DentalHub.Application.Handlers.Students
{
    public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Result<string>>
    {
        private readonly IUserManagementService _userManagementService;

        public CreateStudentCommandHandler(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        public async Task<Result<string>> Handle(CreateStudentCommand request, CancellationToken ct)
        {
            var registerDto = new RegisterStudentDto
            {
                FullName = request.FullName,
                Email = request.Email,
                Password = request.Password,
                UniversityId = request.UniversityId,
                Level = request.Level
            };

            var result = await _userManagementService.RegisterStudentAsync(registerDto);

            if (!result.IsSuccess)
            {
                return Result<string>.Failure(result.Errors ?? new List<string> { result.Message ?? "Student creation failed" }, result.Status);
            }

            return Result<string>.Success(result.Data.PublicId, result.Message, result.Status);
        }
    }
}
