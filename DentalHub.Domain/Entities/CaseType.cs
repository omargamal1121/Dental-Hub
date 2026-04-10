namespace DentalHub.Domain.Entities
{
	public class CaseType :BaseEntitiy
    {
 		public string Name { get; set; }
        public string Description { get; set; }
		public IEnumerable<Diagnosis>   Diagnoses { get; set; }
		public List<Media>   Medias { get; set; }= new List< Media>();
	}
}
