using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DentalHub.Domain.Entities;

namespace DentalHub.Application.DTOs
{public class PatientDto
{
    public Guid UserId { get; set; }
    public int Age { get; set; }
    public string Phone { get; set; }

    public PatientDto(Patient patient)
    {
        UserId = patient.UserId;
        Age = patient.Age;
        Phone = patient.Phone;
    }
}
}

