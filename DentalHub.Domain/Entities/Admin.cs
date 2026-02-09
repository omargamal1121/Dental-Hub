namespace DentalHub.Domain.Entities
{
    public class Admin : BaseEntitiy
    {
        
        public Guid UserId { get; set; }

       


        public string Phone { get; set; }

        public bool IsSuperAdmin { get; set; }


        public User User { get; set; }
    }
}
