using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Queries.Students;
using DentalHub.Application.Services.Students;
using MediatR;

public class GetAvailableCasesForStudentQueryHandler
    : IRequestHandler<GetAvailableCasesForStudentQuery, Result<PagedResult<AvailableCasesDto>>>
{
    private readonly IStudentService _service;

    public GetAvailableCasesForStudentQueryHandler(IStudentService service)
    {
        _service = service;
    }

    public async Task<Result<PagedResult<AvailableCasesDto>>> Handle(
        GetAvailableCasesForStudentQuery request,
        CancellationToken cancellationToken)
    {
        return await _service.GetAvailableCasesForStudentAsync(
            request.StudentPublicId,
            request.PatientName,
            request.CaseType,
            request.Gender,
            request.DiagnosisSource,
            request.SortBy,
            request.IsDescending,
            request.Page,
            request.PageSize);
    }
}

