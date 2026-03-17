using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Students;
using DentalHub.Application.Queries.Students;
using DentalHub.Application.Services.Students;
using MediatR;

namespace DentalHub.Application.Handlers.Students
{
    public class GetStudentByIdQueryHandler : IRequestHandler<GetStudentByIdQuery, Result<StudentDetailsDto>>
    {
        private readonly IStudentService _service;

        public GetStudentByIdQueryHandler(IStudentService service)
        {
            _service = service;
        }

        public async Task<Result<StudentDetailsDto>> Handle(GetStudentByIdQuery request, CancellationToken ct)
        {
            return await _service.GetStudentByIdAsync(request.PublicId);
        }
    }
}
