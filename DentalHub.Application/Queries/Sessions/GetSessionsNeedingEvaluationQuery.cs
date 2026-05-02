using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;
using MediatR;

namespace DentalHub.Application.Queries.Sessions
{
    public record GetSessionsNeedingEvaluationQuery(
        Guid DoctorId, 
        Guid? StudentId = null, 
        Guid? PatientId = null, 
        int Page = 1, 
        int PageSize = 10) : IRequest<Result<PagedResult<SessionDto>>>;
}
