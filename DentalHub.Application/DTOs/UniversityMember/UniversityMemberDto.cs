namespace DentalHub.Application.DTOs.UniversityMember
{
    public class UniversityMemberDto
    {
        public Guid UniversityId { get; set; }
        public string Name { get; set; } = null!;
        public string Department { get; set; } = null!;
        public string Role { get; set; } = null!;
    }
}
