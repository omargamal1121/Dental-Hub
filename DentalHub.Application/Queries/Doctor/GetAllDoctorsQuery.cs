using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Doctors;

namespace DentalHub.Application.Queries.Doctor
{
    public record GetAllDoctorsQuery(int Page = 1, int PageSize = 10, string? Name = null, string? Spec = null) : IRequest<Result<PagedResult<DoctorlistDto>>>;
}
