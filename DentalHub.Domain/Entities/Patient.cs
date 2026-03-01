namespace DentalHub.Domain.Entities
{
    public class Patient : BaseEntitiy
	{
		public Patient()
		{
			
		}
		public Patient(Guid id,string publicid):base(id,publicid)
		{
            UserId =id;
			
		}
		public Guid UserId { get; set; }
        public int Age { get; set; }
        public string Phone { get; set; }
        public User User { get; set; }
        public ICollection<PatientCase> PatientCases { get; set; } = new List<PatientCase>();
        public ICollection<Media> Medias { get; set; } = new List<Media>();
		public  City  City { get; set; }
	}
}
