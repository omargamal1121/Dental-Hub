using DentalHub.Application.Common;
using MediatR;

namespace DentalHub.Application.Commands.Patient
{
  public record DeletePatientCommand(Guid UserId)
    : IRequest<Result<bool>>;
}

