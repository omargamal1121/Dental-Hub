using DentalHub.Application.Services.Admins;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DentalHub.Application.Features.Admins.Commands.DeleteAdmin
{
    /// Handler for deleting an admin
    public class DeleteAdminCommandHandler : IRequestHandler<DeleteAdminCommand, DeleteAdminCommandResponse>
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<DeleteAdminCommandHandler> _logger;

        public DeleteAdminCommandHandler(
            IAdminService adminService,
            ILogger<DeleteAdminCommandHandler> logger)
        {
            _adminService = adminService;
            _logger = logger;
        }

        public async Task<DeleteAdminCommandResponse> Handle(
            DeleteAdminCommand request,
            CancellationToken cancellationToken)
        {
            try
            {
                // Call Service
                var result = await _adminService.DeleteAdminAsync(request.UserId);

                // Convert Result to Response
                return new DeleteAdminCommandResponse
                {
                    Success = result.IsSuccess,
                    Message = result.Message ?? string.Empty,
                    Errors = result.Errors ?? new List<string>()
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteAdminCommandHandler");
                return new DeleteAdminCommandResponse
                {
                    Success = false,
                    Message = "An error occurred while deleting admin",
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}
