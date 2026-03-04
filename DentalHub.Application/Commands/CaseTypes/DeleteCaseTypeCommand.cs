using DentalHub.Application.Common;
using MediatR;

namespace DentalHub.Application.Commands.CaseTypes
{
    public class DeleteCaseTypeCommand : IRequest<Result>
    {
        public Guid Id { get; }

        public DeleteCaseTypeCommand(Guid id)
        {
            Id = id;
        }
    }
}
