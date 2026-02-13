using DentalHub.Application.Commands.CaseTypes;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.CaseTypes;
using DentalHub.Application.Services.CaseTypes;
using MediatR;

namespace DentalHub.Application.Handlers.CaseTypes
{
    public class UpdateCaseTypeCommandHandler : IRequestHandler<UpdateCaseTypeCommand, Result<CaseTypeDto>>
    {
        private readonly ICaseTypeService _service;

        public UpdateCaseTypeCommandHandler(ICaseTypeService service)
        {
            _service = service;
        }

        public async Task<Result<CaseTypeDto>> Handle(UpdateCaseTypeCommand request, CancellationToken cancellationToken)
        {
            return await _service.UpdateCaseTypeAsync(request.ToDto());
        }
    }
}
