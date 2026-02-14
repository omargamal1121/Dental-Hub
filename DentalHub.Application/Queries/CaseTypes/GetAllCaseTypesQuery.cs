using DentalHub.Application.Common;
using DentalHub.Application.DTOs.CaseTypes;
using MediatR;

namespace DentalHub.Application.Queries.CaseTypes
{
    public class GetAllCaseTypesQuery : IRequest<Result<PagedResult<CaseTypeDto>>>
    {
        public int Page { get; }
        public int PageSize { get; }
        public string? Searching { get; }

        public GetAllCaseTypesQuery(int page, int pageSize, string? searching = null)
        {
            Page = page;
            PageSize = pageSize;
            Searching = searching;
        }
    }
}
