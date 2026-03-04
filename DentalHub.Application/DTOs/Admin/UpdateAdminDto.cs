namespace DentalHub.Application.DTOs.Admins
{
    public class UpdateAdminDto
    {
        public Guid PublicId { get; set; }

        public string? FullName { get; set; }

        public string? Role { get; set; }

        public string? Phone { get; set; }

        public bool? IsSuperAdmin { get; set; }
    }
}
