using System;

namespace DentalHub.Domain.Entities
{
    public class Student : BaseEntitiy
	{
		public Student()
		{
			
		}
		public Student(Guid id,string publicid):base(id,publicid) 
		{
			UserId=id;
		}
		public Guid UserId { get; set; }
        public string University { get; set; }
        public int Level { get; set; }
		public string UniversityId { get; set; }

		public ICollection<CaseRequest>  CaseRequests { get; set; }
		public User User { get; set; }
    }
}
