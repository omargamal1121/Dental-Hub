using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Commands.Doctor
{
   public record UpdateDoctorCommand(
    Guid Id,
    string Name,
    string Specialty,
    int UniversityId
) : IRequest<Result>;

}

