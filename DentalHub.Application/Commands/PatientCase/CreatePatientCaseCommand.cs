using MediatR;
using DentalHub.Application.Common;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace DentalHub.Application.Commands.PatientCase
{
    public record CreatePatientCaseCommand(
        Guid PatientId,
        string Title,
        string Description,
        Guid CaseTypeId,
        bool IsPublic = false,
        Guid? UniversityId = null,
        List<IFormFile>? Images = null,
        Guid? CreatedById = null,
        string? CreatedByRole = null
    ) : IRequest<Result<Guid>>;
}
