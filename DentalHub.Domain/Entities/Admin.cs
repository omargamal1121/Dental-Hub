namespace DentalHub.Domain.Entities
{
    public class Admin : BaseEntitiy
    {
		public Admin()
		{
			
		}
		public Admin(Guid id): base(id)
		{

		}
		public string Phone { get; set; }

        public bool IsSuperAdmin { get; set; }


        public User User { get; set; }
    }
}
