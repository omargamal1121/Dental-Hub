using DentalHub.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Commands.Patient
{
    public record CreatePatientCommand(
    Guid UserId,
    int Age,
    string Phone
) : IRequest<Result<Guid>>;
}

