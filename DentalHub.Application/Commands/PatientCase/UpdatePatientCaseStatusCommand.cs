using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.PatientCase
{
    public class UpdatePatientCaseStatusCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }
        public string Status { get; set; }

        public UpdatePatientCaseStatusCommand(Guid id, string status)
        {
            Id = id;
            Status = status;
        }
    }
}
