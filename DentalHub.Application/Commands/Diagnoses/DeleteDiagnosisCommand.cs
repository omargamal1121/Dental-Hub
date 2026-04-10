using DentalHub.Application.Common;
using MediatR;

namespace DentalHub.Application.Commands.Diagnoses
{
    public class DeleteDiagnosisCommand : IRequest<Result>
    {
        public Guid Id { get; set; }

        public DeleteDiagnosisCommand(Guid id)
        {
            Id = id;
        }
    }
}
