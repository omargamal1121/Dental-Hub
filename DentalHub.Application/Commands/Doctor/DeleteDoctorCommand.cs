using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Doctor
{
    public record DeleteDoctorCommand(Guid PublicId) : IRequest<Result<bool>>;
}
