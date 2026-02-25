using MediatR;
using DentalHub.Application.Common;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace DentalHub.Application.Commands.PatientCase
{
    public record CreatePatientCaseCommand(
        string PatientId,
        string Title,
        string Description,
        string CaseTypeId,
        List<IFormFile>? Images = null
    ) : IRequest<Result<string>>;
}
