using DentalHub.Application.Commands.PatientCase;
using DentalHub.Application.Common;
using DentalHub.Application.Services.Cases;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace DentalHub.Application.Handlers.PatientCase
{
    public class AssignCaseUniversityCommandHandler : IRequestHandler<AssignCaseUniversityCommand, Result>
    {
        private readonly IPatientCaseService _service;

        public AssignCaseUniversityCommandHandler(IPatientCaseService service)
        {
            _service = service;
        }

        public async Task<Result> Handle(AssignCaseUniversityCommand request, CancellationToken cancellationToken)
        {
            return await _service.AssignUniversityAsync(request.PatientCaseId, request.UniversityId, request.IsPublic, request.UserId, request.Role);
        }
    }
}
