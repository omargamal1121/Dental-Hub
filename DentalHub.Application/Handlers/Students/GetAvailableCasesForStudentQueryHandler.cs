using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Queries.Students;
using DentalHub.Application.Services.Students;
using MediatR;

namespace DentalHub.Application.Handlers.Students
{
    public class GetAvailableCasesForStudentQueryHandler : IRequestHandler<GetAvailableCasesForStudentQuery, Result<List<PatientCaseDto>>>
    {
        private readonly IStudentService _service;

        public GetAvailableCasesForStudentQueryHandler(IStudentService service)
        {
            _service = service;
        }

        public async Task<Result<List<PatientCaseDto>>> Handle(GetAvailableCasesForStudentQuery request, CancellationToken ct)
        {
            return await _service.GetAvailableCasesForStudentAsync(request.StudentId, request.Page, request.PageSize);
        }
    }
}
