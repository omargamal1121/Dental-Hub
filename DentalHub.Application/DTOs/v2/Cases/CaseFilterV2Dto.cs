using DentalHub.Application.DTOs.Cases;

namespace DentalHub.Application.DTOs.v2.Cases
{
    public class CaseFilterV2Dto : CaseFilterDto
    {
        public string? NationalId { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
