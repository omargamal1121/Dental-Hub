using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Students
{
    public record DeleteStudentCommand(Guid Id) : IRequest<Result<bool>>;
}
