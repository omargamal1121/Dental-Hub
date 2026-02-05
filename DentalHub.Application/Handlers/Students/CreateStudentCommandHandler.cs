using DentalHub.Application.Commands.Students;
using DentalHub.Application.Common;
using DentalHub.Application.Services.Identity;
using DentalHub.Application.DTOs.Identity;
using MediatR;

namespace DentalHub.Application.Handlers.Students
{
    public class CreateStudentCommandHandler : IRequestHandler<CreateStudentCommand, Result<Guid>>
    {
        private readonly IUserManagementService _userManagementService;

        public CreateStudentCommandHandler(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        public async Task<Result<Guid>> Handle(CreateStudentCommand request, CancellationToken ct)
        {
            var registerDto = new RegisterStudentDto
            {
                FullName = request.FullName,
                Email = request.Email,
                Password = request.Password,
                University = request.University,
                
            };

            var result = await _userManagementService.RegisterStudentAsync(registerDto);

            if (!result.IsSuccess)
            {
                return Result<Guid>.Failure(result.Errors ?? new List<string> { result.Message ?? "Student creation failed" }, result.Status);
            }

            return Result<Guid>.Success(result.Data.UserId, result.Message, result.Status);
        }
    }
}
