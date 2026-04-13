using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;

//namespace DentalHub.Application.Queries.Students
//{
//    /// <summary>
//    /// Returns all cases on which the current student has already placed a request.
//    /// The student PublicId is resolved in the controller from the JWT token.
//    /// </summary>
//    public record GetMyCasesForStudentQuery(
//		Guid StudentPublicId,
//        string? Casetype = null,
//		int Page     = 1,
//        int PageSize = 10
//    ) : IRequest<Result<PagedResult<PatientCaseDto>>>;
//}
public class GetMyCasesForStudentQuery
    : IRequest<Result<PagedResult<PatientCaseDto>>>
{
    public Guid StudentPublicId { get; }
    public CaseStatus? Status { get; }
    public int Page { get; }
    public int PageSize { get; }

    public GetMyCasesForStudentQuery(
        Guid studentPublicId,
        CaseStatus? status,
        int page,
        int pageSize)
    {
        StudentPublicId = studentPublicId;
        Status = status;
        Page = page;
        PageSize = pageSize;
    }
} 


