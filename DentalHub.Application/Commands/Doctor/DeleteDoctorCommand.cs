using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Doctor
{
    public record DeleteDoctorCommand(string PublicId) : IRequest<Result<bool>>;
}
