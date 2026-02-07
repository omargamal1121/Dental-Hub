using DentalHub.Application.DTOs.Admins;
using MediatR;

namespace DentalHub.Application.Features.Admins.Commands.UpdateAdmin
{
    /// Command to update admin information
    public class UpdateAdminCommand : IRequest<UpdateAdminCommandResponse>
    {
        public Guid UserId { get; set; }
        public string? FullName { get; set; }
        public string? Role { get; set; }
        public string? Phone { get; set; }
        public bool? IsSuperAdmin { get; set; }
    }

    /// Response for update admin command
    public class UpdateAdminCommandResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public AdminDto? Admin { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
