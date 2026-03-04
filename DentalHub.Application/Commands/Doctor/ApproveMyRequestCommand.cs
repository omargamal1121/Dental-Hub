using DentalHub.Application.Common;
using MediatR;

namespace DentalHub.Application.Commands.Doctor
{
    /// Command for a doctor to approve a case request.
    /// DoctorId comes from JWT token (enforced in controller).
    public record ApproveMyRequestCommand(Guid RequestId, Guid DoctorUserId)
        : IRequest<Result<bool>>;

    /// Command for a doctor to reject a case request.
    public record RejectMyRequestCommand(Guid RequestId, Guid DoctorUserId)
        : IRequest<Result<bool>>;
}
