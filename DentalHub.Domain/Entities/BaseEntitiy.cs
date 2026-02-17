using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Domain.Entities
{
	public abstract class BaseEntitiy
	{
		public Guid Id { get; set; } = Guid.CreateVersion7();
		public string PublicId { get; set; }

		public BaseEntitiy()
		{
			PublicId = DentalHub.Domain.Utils.Base62Converter.Encode(Id);
		}

		public DateTime CreateAt { get; set; } = DateTime.UtcNow;
		public DateTime? UpdateAt { get; set; }
		public DateTime? DeleteAt { get; set; }
	}
}
