using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Sessions
{
    public record DeleteSessionCommand(Guid Id) : IRequest<Result<bool>>;
}
