using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Sessions
{
    public record DeleteSessionCommand(string Id) : IRequest<Result<bool>>;
}
