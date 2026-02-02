namespace DentalHub.Application.DTOs.Identity
{
    /// <summary>
    /// Response after successful login/registration
    /// البيانات اللي بترجع بعد تسجيل دخول أو تسجيل جديد ناجح
    /// </summary>
    public class AuthResponseDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string? Token { get; set; } // للـ JWT في المستقبل
    }
}
