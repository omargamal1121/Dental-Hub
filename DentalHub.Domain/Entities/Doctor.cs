namespace DentalHub.Domain.Entities
{
    public class Doctor:BaseEntitiy 
    {
		public Doctor()
		{
			
		}
		public Doctor(Guid id):base(id)
		{
			
		}
		public string Name {  get; set; }
     
        public string Specialty { get; set; }
		public Guid UniversityId { get; set; }
        public University University { get; set; } = null!;
        public User User { get; set; }
		public List<CaseRequest>  CaseRequests { get; set; }
	}
}
