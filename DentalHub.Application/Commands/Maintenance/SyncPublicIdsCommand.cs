using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Maintenance
{
    public record SyncPublicIdsCommand() : IRequest<Result<string>>;
}
