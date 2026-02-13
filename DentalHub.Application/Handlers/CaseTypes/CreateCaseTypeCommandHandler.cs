using DentalHub.Application.Commands.CaseTypes;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.CaseTypes;
using DentalHub.Application.Services.CaseTypes;
using MediatR;

namespace DentalHub.Application.Handlers.CaseTypes
{
    public class CreateCaseTypeCommandHandler : IRequestHandler<CreateCaseTypeCommand, Result<CaseTypeDto>>
    {
        private readonly ICaseTypeService _service;

        public CreateCaseTypeCommandHandler(ICaseTypeService service)
        {
            _service = service;
        }

        public async Task<Result<CaseTypeDto>> Handle(CreateCaseTypeCommand request, CancellationToken cancellationToken)
        {
            return await _service.CreateCaseTypeAsync(request.ToDto());
        }
    }
}
