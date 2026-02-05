using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Students;
using DentalHub.Application.Queries.Students;
using DentalHub.Application.Services.Students;
using MediatR;

namespace DentalHub.Application.Handlers.Students
{
    public class GetAllStudentsQueryHandler : IRequestHandler<GetAllStudentsQuery, Result<List<StudentDto>>>
    {
        private readonly IStudentService _service;

        public GetAllStudentsQueryHandler(IStudentService service)
        {
            _service = service;
        }

        public async Task<Result<List<StudentDto>>> Handle(GetAllStudentsQuery request, CancellationToken ct)
        {
            return await _service.GetAllStudentsAsync(request.Page, request.PageSize);
        }
    }
}
