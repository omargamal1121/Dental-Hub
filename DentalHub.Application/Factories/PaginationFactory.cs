using DentalHub.Application.Common;

namespace DentalHub.Application.Factories
{
	public static class PaginationFactory<T> where T : class
	{
		public static PagedResult<T> Create(
			int count,
			int page,
			int pageSize,
			List<T> data)
		{
			var totalPages = (int)Math.Ceiling((double)count / pageSize);

			return new PagedResult<T>
			{
				CurrentPage = page,
				Items = data,
				TotalCount = count,
				TotalPages = totalPages,
				HasPreviousPage = page > 1,
				HasNextPage = page < totalPages
			};
		}
	}

}
