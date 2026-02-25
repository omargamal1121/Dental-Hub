using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Queries.Students;
using DentalHub.Application.Services.Students;
using MediatR;

namespace DentalHub.Application.Handlers.Students
{
    public class GetMyCasesForStudentQueryHandler
        : IRequestHandler<GetMyCasesForStudentQuery, Result<PagedResult<PatientCaseDto>>>
    {
        private readonly IStudentService _service;

        public GetMyCasesForStudentQueryHandler(IStudentService service)
        {
            _service = service;
        }

        public async Task<Result<PagedResult<PatientCaseDto>>> Handle(
            GetMyCasesForStudentQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetMyCasesForStudentAsync(
                request.StudentPublicId,
                request.Casetype,
                request.Page,
                request.PageSize);
        }
    }
}
