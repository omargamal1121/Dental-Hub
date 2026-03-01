using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Queries.Students;
using DentalHub.Application.Services.Students;
using MediatR;

namespace DentalHub.Application.Handlers.Students
{
    public class GetAvailableCasesForStudentQueryHandler : IRequestHandler<GetAvailableCasesForStudentQuery, Result<PagedResult<AvailableCasesDto>>>
    {
        private readonly IStudentService _service;

        public GetAvailableCasesForStudentQueryHandler(IStudentService service)
        {
            _service = service;
        }

        public async Task<Result<PagedResult<AvailableCasesDto>>> Handle(GetAvailableCasesForStudentQuery request, CancellationToken ct)
        {
            return await _service.GetAvailableCasesForStudentAsync(request.StudentPublicId,request.CaseType ,request.PageNumber, request.PageSize);
        }
    }
}
