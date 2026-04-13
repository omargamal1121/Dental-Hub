using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;

namespace DentalHub.Application.Queries.PatientCase
{
    /// <param name="Id">Case public ID</param>
    /// <param name="UserId">Current user ID (nullable for anonymous/admin access)</param>
    /// <param name="UserRole">Current user role: Patient | Student | Doctor | Admin</param>
    public record GetPatientCaseByIdQuery(Guid Id, Guid? UserId = null, string? UserRole = null)
        : IRequest<Result<PatientCaseDto>>;
}
