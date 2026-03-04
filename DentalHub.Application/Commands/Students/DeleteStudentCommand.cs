using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Students
{
    public record DeleteStudentCommand(Guid PublicId) : IRequest<Result<bool>>;
}
