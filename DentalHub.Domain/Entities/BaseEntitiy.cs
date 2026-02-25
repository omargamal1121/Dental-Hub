namespace DentalHub.Domain.Entities
{
	public abstract class BaseEntitiy
	{
		public Guid Id { get; set; } 
		public string PublicId { get; set; }

		public BaseEntitiy()
		{
			Id = Guid.CreateVersion7();
			PublicId = Utils.Base62Converter.Encode(Id);
		}
		public BaseEntitiy(Guid id,string publicid)
		{
			Id=id;
			PublicId =publicid;


		}

		public DateTime CreateAt { get; set; } = DateTime.UtcNow;
		public DateTime? UpdateAt { get; set; }
		public DateTime? DeleteAt { get; set; }
	}
}
