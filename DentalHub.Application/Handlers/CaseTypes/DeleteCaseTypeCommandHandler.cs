using DentalHub.Application.Commands.CaseTypes;
using DentalHub.Application.Common;
using DentalHub.Application.Services.CaseTypes;
using MediatR;

namespace DentalHub.Application.Handlers.CaseTypes
{
    public class DeleteCaseTypeCommandHandler : IRequestHandler<DeleteCaseTypeCommand, Result>
    {
        private readonly ICaseTypeService _service;

        public DeleteCaseTypeCommandHandler(ICaseTypeService service)
        {
            _service = service;
        }

        public async Task<Result> Handle(DeleteCaseTypeCommand request, CancellationToken cancellationToken)
        {
            return await _service.DeleteCaseTypeAsync(request.Id);
        }
    }
}
