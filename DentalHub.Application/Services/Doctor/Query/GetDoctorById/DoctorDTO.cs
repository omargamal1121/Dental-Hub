using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Services.Doctor.Query.GetDoctorById
{
    public record DoctorDto(
     int Id,
     string Name,
     string Specialization
 );
}
