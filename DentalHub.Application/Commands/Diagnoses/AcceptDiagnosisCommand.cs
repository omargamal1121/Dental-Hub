using DentalHub.Application.Common;
using MediatR;

namespace DentalHub.Application.Commands.Diagnoses
{
    public class AcceptDiagnosisCommand : IRequest<Result>
    {
        public Guid Id { get; set; }

        public AcceptDiagnosisCommand(Guid id)
        {
            Id = id;
        }
    }
}
