using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DentalHub.Application.Common;
using MediatR;

namespace DentalHub.Application.Commands.Doctor
{
    /// <summary>
    /// Changed return type from Result<Guid> to Result<bool> for consistency
    /// All Delete commands should return the same type
    /// </summary>
    public record DeleteDoctorCommand(Guid Id)
        : IRequest<Result<bool>>;
}
