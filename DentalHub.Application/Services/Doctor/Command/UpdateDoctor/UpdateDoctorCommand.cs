using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Services.Doctor.Command.UpdateDoctor
{
    public record UpdateDoctorCommand(int Id, string Name, string Specialization);

}
