using DentalHub.Application.DTOs.Admins;
using MediatR;

namespace DentalHub.Application.Features.Admins.Queries.GetAllAdmins
{
    /// Query to get all admins with pagination
    public class GetAllAdminsQuery : IRequest<GetAllAdminsQueryResponse>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    /// Response for get all admins query
    public class GetAllAdminsQueryResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<AdminDto> Admins { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }
}
