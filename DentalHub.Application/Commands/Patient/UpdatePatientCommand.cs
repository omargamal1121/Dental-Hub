using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Commands.Patient
{
    public record UpdatePatientCommand(
    Guid UserId,
    int Age,
    string Phone
) : IRequest<Result>;
}

