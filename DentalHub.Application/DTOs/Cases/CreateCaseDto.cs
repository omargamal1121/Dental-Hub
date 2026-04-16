using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace DentalHub.Application.DTOs.Cases
{
    /// DTO for creating a new patient case
    public class CreateCaseDto
    {
        public Guid PatientId { get; set; } 

		public Guid CaseTypeId { get; set; } 


        public string? Description { get; set; }
        public bool IsPublic { get; set; }
        public Guid? UniversityId { get; set; }

        public List<IFormFile>? Images { get; set; }
        public Guid? CreatedById { get; set; }
        public string? CreatedByRole { get; set; }

        public string? Phone { get; set; }
        public string? NationalId { get; set; }
        public string? City { get; set; }

    }
}
