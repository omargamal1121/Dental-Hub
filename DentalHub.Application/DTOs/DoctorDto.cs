using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DentalHub.Domain.Entities;
namespace DentalHub.Application.DTOs
{
   public class DoctorDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Specialty { get; set; }
    public int UniversityId { get; set; }

    public DoctorDto(Doctor doctor)
    {
        Id = doctor.UserId;
        Name = doctor.Name;
        Specialty = doctor.Specialty;
        UniversityId = doctor.UniversityId;
    }
}

}

