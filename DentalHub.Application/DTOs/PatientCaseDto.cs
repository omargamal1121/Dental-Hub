using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DentalHub.Domain.Entities;
namespace DentalHub.Application.DTOs
{
   public class PatientCaseDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string TreatmentType { get; set; }
    public CaseStatus Status { get; set; }

    public PatientCaseDto(PatientCase patientCase)
    {
        Id = patientCase.Id;
        PatientId = patientCase.PatientId;
        TreatmentType = patientCase.TreatmentType;
        Status = patientCase.Status;
    }
}

}

