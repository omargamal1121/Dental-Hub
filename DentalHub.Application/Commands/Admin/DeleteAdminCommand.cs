using MediatR;

namespace DentalHub.Application.Features.Admins.Commands.DeleteAdmin
{
    /// Command to delete an admin
    public class DeleteAdminCommand : IRequest<DeleteAdminCommandResponse>
    {
        public Guid UserId { get; set; }
    }

    /// Response for delete admin command
    public class DeleteAdminCommandResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Errors { get; set; } = new();
    }
}
