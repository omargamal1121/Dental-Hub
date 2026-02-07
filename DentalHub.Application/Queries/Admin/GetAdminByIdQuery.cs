using DentalHub.Application.DTOs.Admins;
using MediatR;

namespace DentalHub.Application.Features.Admins.Queries.GetAdminById
{
    /// Query to get admin by ID
    public class GetAdminByIdQuery : IRequest<GetAdminByIdQueryResponse>
    {
        public Guid UserId { get; set; }
    }

    /// Response for get admin by ID query
    public class GetAdminByIdQueryResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public AdminDto? Admin { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
