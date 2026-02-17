using DentalHub.Application.Common;
using MediatR;

namespace DentalHub.Application.Commands.CaseTypes
{
    public class DeleteCaseTypeCommand : IRequest<Result>
    {
        public string Id { get; }

        public DeleteCaseTypeCommand(string id)
        {
            Id = id;
        }
    }
}
