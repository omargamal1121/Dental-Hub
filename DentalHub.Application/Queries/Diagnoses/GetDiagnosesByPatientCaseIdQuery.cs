using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Diagnoses;
using MediatR;

namespace DentalHub.Application.Queries.Diagnoses
{
    public class GetDiagnosesByPatientCaseIdQuery : IRequest<Result<PagedResult<DiagnosisDto>>>
    {
        public Guid PatientCaseId { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }

        public GetDiagnosesByPatientCaseIdQuery(Guid patientCaseId, int page = 1, int pageSize = 10)
        {
            PatientCaseId = patientCaseId;
            Page = page;
            PageSize = pageSize;
        }
    }
}
