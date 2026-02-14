namespace DentalHub.Application.Common
{
	public class PagedResult<T>where T : class
	{
		public int TotalCount { get; set; }
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
		public bool HasPreviousPage { get; set; }
		public bool HasNextPage { get; set; }

		public List<T> Items { get; set; } = new();

	}
}
