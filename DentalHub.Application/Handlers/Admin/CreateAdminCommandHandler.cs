using DentalHub.Application.DTOs.Admins;
using DentalHub.Application.Services.Admins;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DentalHub.Application.Features.Admins.Commands.CreateAdmin
{
    /// Handler for creating a new admin
    public class CreateAdminCommandHandler : IRequestHandler<CreateAdminCommand, CreateAdminCommandResponse>
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<CreateAdminCommandHandler> _logger;

        public CreateAdminCommandHandler(
            IAdminService adminService,
            ILogger<CreateAdminCommandHandler> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        public async Task<CreateAdminCommandResponse> Handle(
            CreateAdminCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Convert Command to DTO
                var registerDto = new RegisterAdminDto
                {
                    FullName = request.FullName,
                    Email = request.Email,
                    Password = request.Password,
                    Phone = request.Phone,
                    IsSuperAdmin = request.IsSuperAdmin
                };

                // Call Service
                var result = await _adminService.RegisterAdminAsync(registerDto);

                // Convert Result to Response
                return new CreateAdminCommandResponse
                {
                    Success = result.IsSuccess,
                    Message = result.Message ?? string.Empty,
                    Admin = result.Data,
                    Errors = result.Errors ?? new List<string>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateAdminCommandHandler");
                return new CreateAdminCommandResponse
                {
                    Success = false,
                    Message = "An error occurred while creating admin",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}
