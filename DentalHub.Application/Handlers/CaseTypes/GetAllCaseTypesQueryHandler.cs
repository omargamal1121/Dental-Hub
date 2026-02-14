using DentalHub.Application.Common;
using DentalHub.Application.DTOs.CaseTypes;
using DentalHub.Application.Queries.CaseTypes;
using DentalHub.Application.Services.CaseTypes;
using MediatR;

namespace DentalHub.Application.Handlers.CaseTypes
{
    public class GetAllCaseTypesQueryHandler : IRequestHandler<GetAllCaseTypesQuery, Result<PagedResult<CaseTypeDto>>>
    {
        private readonly ICaseTypeService _service;

        public GetAllCaseTypesQueryHandler(ICaseTypeService service)
        {
            _service = service;
        }

        public async Task<Result<PagedResult<CaseTypeDto>>> Handle(GetAllCaseTypesQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetAllCaseTypesAsync(request.Page, request.PageSize, request.Searching);
        }
    }
}
