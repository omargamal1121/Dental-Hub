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
        List<IFormFile>? Images = null
    ) : IRequest<Result<Guid>>;
}
