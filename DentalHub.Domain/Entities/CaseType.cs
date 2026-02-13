namespace DentalHub.Domain.Entities
{
	public class CaseType :BaseEntitiy
    {
        public Guid Id { get; set; }
		public string Name { get; set; }
        public string Description { get; set; }
		public List<PatientCase>  PatientCases { get; set; }= new List<PatientCase>();
		public List<Media>   Medias { get; set; }= new List< Media>();
	}
}
