using DentalHub.Application.Common;
using DentalHub.Application.DTOs.CaseTypes;
using DentalHub.Application.Queries.CaseTypes;
using DentalHub.Application.Services.CaseTypes;
using MediatR;

namespace DentalHub.Application.Handlers.CaseTypes
{
    public class GetCaseTypeByIdQueryHandler : IRequestHandler<GetCaseTypeByIdQuery, Result<CaseTypeDto>>
    {
        private readonly ICaseTypeService _service;

        public GetCaseTypeByIdQueryHandler(ICaseTypeService service)
        {
            _service = service;
        }

        public async Task<Result<CaseTypeDto>> Handle(GetCaseTypeByIdQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetCaseTypeByIdAsync(request.Id);
        }
    }
}
