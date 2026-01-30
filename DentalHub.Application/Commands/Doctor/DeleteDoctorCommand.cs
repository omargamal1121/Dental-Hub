using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Commands.Doctor
{
   public record DeleteDoctorCommand(Guid Id)
    : IRequest<Result>;

}

