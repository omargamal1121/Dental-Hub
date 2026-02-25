using DentalHub.Application.Commands.PatientCase;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Services.Cases;
using MediatR;

namespace DentalHub.Application.Handlers.PatientCase
{
    public class CreatePatientCaseCommandHandler : IRequestHandler<CreatePatientCaseCommand, Result<string>>
    {
        private readonly IPatientCaseService _service;

        public CreatePatientCaseCommandHandler(IPatientCaseService service)
        {
            _service = service;
        }

        public async Task<Result<string>> Handle(CreatePatientCaseCommand request, CancellationToken ct)
        {
            var dto = new CreateCaseDto
            {
                PatientId = request.PatientId,
                CaseTypeId= request.CaseTypeId,
                
              
                Description = request.Description,
                Images = request.Images
            };

            var result = await _service.CreateCaseAsync(dto);

            if (!result.IsSuccess)
            {
                return Result<string>.Failure(result.Errors ?? new List<string> { result.Message ?? "Case creation failed" }, result.Status);
            }

            return Result<string>.Success(result.Data.Id, result.Message, result.Status);
        }
    }
}
