using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Services.Doctor.Command.CreateDoctor
{
    public record CreateDoctorDto(
       string Name,
       string Specialization
   );
}
