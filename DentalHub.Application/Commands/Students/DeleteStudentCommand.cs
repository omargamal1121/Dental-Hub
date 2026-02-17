using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Students
{
    public record DeleteStudentCommand(string PublicId) : IRequest<Result<bool>>;
}
