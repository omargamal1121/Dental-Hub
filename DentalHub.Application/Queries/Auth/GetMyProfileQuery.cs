using DentalHub.Application.Common;
using MediatR;

namespace DentalHub.Application.Queries.Auth
{
    /// Query to get the current logged-in user's profile.
    /// Works for Doctor, Student, Patient, and Admin roles.
    public record GetMyProfileQuery(string UserId, string Role) : IRequest<Result<object>>;
}
