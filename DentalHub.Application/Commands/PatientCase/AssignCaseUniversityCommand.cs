using DentalHub.Application.Common;
using MediatR;
using System;

namespace DentalHub.Application.Commands.PatientCase
{
    public record AssignCaseUniversityCommand(
        Guid PatientCaseId,
        Guid? UniversityId, // Nullable, as Student/Doctor will be mapped automatically
        bool IsPublic,
        Guid UserId,
        string Role
    ) : IRequest<Result>;
}
