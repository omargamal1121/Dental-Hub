using DentalHub.Application.DTOs.Admins;
using DentalHub.Application.Services.Admins;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DentalHub.Application.Features.Admins.Commands.UpdateAdmin
{
    /// Handler for updating admin information
    public class UpdateAdminCommandHandler : IRequestHandler<UpdateAdminCommand, UpdateAdminCommandResponse>
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<UpdateAdminCommandHandler> _logger;

        public UpdateAdminCommandHandler(
            IAdminService adminService,
            ILogger<UpdateAdminCommandHandler> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        public async Task<UpdateAdminCommandResponse> Handle(
            UpdateAdminCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Convert Command to DTO
                var updateDto = new UpdateAdminDto
                {
                    UserId = request.UserId,
                    FullName = request.FullName,
                    Role = request.Role,
                    Phone = request.Phone,
                    IsSuperAdmin = request.IsSuperAdmin
                };

                // Call Service
                var result = await _adminService.UpdateAdminAsync(updateDto);

                // Convert Result to Response
                return new UpdateAdminCommandResponse
                {
                    Success = result.IsSuccess,
                    Message = result.Message ?? string.Empty,
                    Admin = result.Data,
                    Errors = result.Errors ?? new List<string>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAdminCommandHandler");
                return new UpdateAdminCommandResponse
                {
                    Success = false,
                    Message = "An error occurred while updating admin",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}
