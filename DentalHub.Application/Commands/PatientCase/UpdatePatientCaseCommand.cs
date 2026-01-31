using DentalHub.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Commands.PatientCase
{
 public record UpdatePatientCaseCommand(
    Guid Id,
    string TreatmentType,
    CaseStatus Status
) : IRequest<Result<bool>>;

}

