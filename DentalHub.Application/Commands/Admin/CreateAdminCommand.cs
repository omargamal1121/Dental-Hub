using DentalHub.Application.DTOs.Admins;
using MediatR;

namespace DentalHub.Application.Features.Admins.Commands.CreateAdmin
{
    /// Command to create a new admin
    public class CreateAdminCommand : IRequest<CreateAdminCommandResponse>
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public bool IsSuperAdmin { get; set; } = false;
    }

    /// Response for create admin command
    public class CreateAdminCommandResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public AdminDto? Admin { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
