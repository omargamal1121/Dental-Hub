namespace DentalHub.Domain.Entities
{
    /// Represents an administrator in the system
    /// Admin can manage all users, cases, and system settings
    public class Admin : BaseEntitiy
    {
        /// Foreign key to User table
        public Guid UserId { get; set; }

        /// Admin's role or department (e.g., "System Admin", "Support Admin")
        public string Role { get; set; }

        /// Admin's phone number for contact
        public string Phone { get; set; }

        /// Whether this admin has super admin privileges
        public bool IsSuperAdmin { get; set; }

        /// Navigation property to User
        public User User { get; set; }
    }
}
