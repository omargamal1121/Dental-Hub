using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DentalHub.Application.Common;
using MediatR;
namespace DentalHub.Application.Commands.Doctor
{
   public record DeleteDoctorCommand(Guid Id)
    : IRequest<Result<Guid>>;

}

