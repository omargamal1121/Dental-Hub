using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace DentalHub.Application.DTOs.Cases
{
    /// DTO for creating a new patient case
    public class CreateCaseDto
    {
        public string PatientId { get; set; } = string.Empty;

		public string CaseTypeId { get; set; } = string.Empty;


        public string? Description { get; set; }

        public List<IFormFile>? Images { get; set; }
    }
}
