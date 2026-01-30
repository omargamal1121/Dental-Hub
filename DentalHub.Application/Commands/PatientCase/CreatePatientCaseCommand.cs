using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Commands.PatientCase
{
 public record CreatePatientCaseCommand(
    Guid PatientId,
    string TreatmentType
) : IRequest<Result<Guid>>;

}

