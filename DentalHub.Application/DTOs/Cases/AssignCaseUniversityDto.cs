using System;

namespace DentalHub.Application.DTOs.Cases
{
    public class AssignCaseUniversityDto
    {
        public Guid? UniversityId { get; set; }
        public bool IsPublic { get; set; } = false;
    }
}
