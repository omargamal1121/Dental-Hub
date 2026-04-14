using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;

//namespace DentalHub.Application.Queries.Students
//{
//    public record GetAvailableCasesForStudentQuery(
//		Guid StudentPublicId,
//        string? CaseType = null,
//		int PageNumber = 1,
//        int PageSize = 10
//    ) : IRequest<Result<PagedResult<AvailableCasesDto>>>;
//}

public class GetAvailableCasesForStudentQuery
    : IRequest<Result<PagedResult<AvailableCasesDto>>>
{
<<<<<<< HEAD
    public Guid StudentPublicId { get; }

    public string? PatientName { get; }
    public string? CaseType { get; }

    public Gender? Gender { get; }
    public DiagnosisSource? DiagnosisSource { get; }

    public CaseSortBy? SortBy { get; }
    public bool IsDescending { get; }

    public int Page { get; }
    public int PageSize { get; }

    public GetAvailableCasesForStudentQuery(
        Guid studentPublicId,
        string? patientName,
        string? caseType,
        Gender? gender,
        DiagnosisSource? diagnosisSource,
        CaseSortBy? sortBy,
        bool isDescending,
        int page,
        int pageSize)
    {
        StudentPublicId = studentPublicId;
        PatientName = patientName;
        CaseType = caseType;
        Gender = gender;
        DiagnosisSource = diagnosisSource;
        SortBy = sortBy;
        IsDescending = isDescending;
        Page = page;
        PageSize = pageSize;
    }
=======
    public record GetAvailableCasesForStudentQuery(
		Guid StudentPublicId,
        CaseFilterDto Filter
    ) : IRequest<Result<PagedResult<AvailableCasesDto>>>;
>>>>>>> 06fab53184b37dc7805f290a4acd8dead9fd536c
}

