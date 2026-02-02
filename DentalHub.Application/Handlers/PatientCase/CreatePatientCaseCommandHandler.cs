using DentalHub.Application.Commands.PatientCase;
using DentalHub.Application.Common;
using DentalHub.Application.Services.Cases;
using DentalHub.Application.DTOs.Cases;
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
                TreatmentType = request.TreatmentType
            };

            var result = await _service.CreateCaseAsync(dto);

            if (!result.IsSuccess)
            {
                return Result<Guid>.Failure(result.Message ?? "Creation failed");
            }

            return Result<Guid>.Success(result.Data.Id, "Case created successfully");
        }
    }
}
