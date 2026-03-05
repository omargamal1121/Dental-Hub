using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.PatientCase
{
    public class UpdatePatientCaseCommand : IRequest<Result<bool>>
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public Guid CaseTypeId { get; set; }
        public string? Status { get; set; }
    }
}
