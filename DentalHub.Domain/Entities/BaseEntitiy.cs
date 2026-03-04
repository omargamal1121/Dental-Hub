namespace DentalHub.Domain.Entities
{
	public abstract class BaseEntitiy
	{
		public Guid Id { get; set; } 
	
		public BaseEntitiy()
		{
			Id = Guid.CreateVersion7();
		
		}
		public BaseEntitiy(Guid id)
		{
			Id=id;
		}

		public DateTime CreateAt { get; set; } = DateTime.UtcNow;
		public DateTime? UpdateAt { get; set; }
		public DateTime? DeleteAt { get; set; }
	}
}
