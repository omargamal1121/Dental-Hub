using DentalHub.Application.Common;
using MediatR;

namespace DentalHub.Application.Commands.Sessions
{
    public record UpdateSessionScheduleCommand : IRequest<Result<bool>>
    {
        public Guid SessionId { get; set; }
        public Guid StudentId { get; set; }
        public DateTime NewScheduledAt { get; set; }

        public UpdateSessionScheduleCommand() { }

        public UpdateSessionScheduleCommand(Guid sessionId, DateTime newScheduledAt)
        {
            SessionId = sessionId;
            NewScheduledAt = newScheduledAt;
        }
    }
}

