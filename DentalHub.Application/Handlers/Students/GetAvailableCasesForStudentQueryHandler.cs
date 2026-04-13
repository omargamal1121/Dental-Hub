using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Queries.Students;
using DentalHub.Application.Services.Students;
using MediatR;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace DentalHub.Application.Handlers.Students
{
    public class GetAvailableCasesForStudentQueryHandler
        : IRequestHandler<GetAvailableCasesForStudentQuery, Result<PagedResult<AvailableCasesDto>>>
    {
        private readonly ICaseService _service;

        public GetAvailableCasesForStudentQueryHandler(ICaseService service)
        {
            _service = service;
        }

        public async Task<Result<PagedResult<AvailableCasesDto>>> Handle(
            GetAvailableCasesForStudentQuery request,
            CancellationToken cancellationToken)
        {
            var data = await _service.GetAvailableCasesAsync(
                request.StudentPublicId,
                request.PatientName,
                request.CaseType,
                request.Gender,
                request.DiagnosisSource,
                request.SortBy,
                request.IsDescending,
                request.Page,
                request.PageSize
            );

            return Result<PagedResult<AvailableCasesDto>>.Success(data);

        }
    }


}
