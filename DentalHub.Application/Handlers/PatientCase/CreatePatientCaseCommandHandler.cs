using DentalHub.Application.Commands.PatientCase;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Services.Cases;
using MediatR;

namespace DentalHub.Application.Handlers.PatientCase
{
    public class CreatePatientCaseCommandHandler : IRequestHandler<CreatePatientCaseCommand, Result<Guid>>
    {
        private readonly IPatientCaseService _service;

        public CreatePatientCaseCommandHandler(IPatientCaseService service)
        {
            _service = service;
        }

        public async Task<Result<Guid>> Handle(CreatePatientCaseCommand request, CancellationToken ct)
        {
            var dto = new CreateCaseDto
            {
                PatientId = request.PatientId,
              
                Description = request.Description,
             
            };

            var result = await _service.CreateCaseAsync(dto);

            if (!result.IsSuccess)
            {
                return Result<Guid>.Failure(result.Errors ?? new List<string> { result.Message ?? "Case creation failed" }, result.Status);
            }

            return Result<Guid>.Success(result.Data.Id, result.Message, result.Status);
        }
    }
}
