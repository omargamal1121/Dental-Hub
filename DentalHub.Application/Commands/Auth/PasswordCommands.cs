using DentalHub.Application.DTOs.Shared;
using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Commands.Auth
{
    public record ForgotPasswordCommand(string Email) : IRequest<Result<bool>>;
    public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest<Result<bool>>;
    // ChangePasswordCommand often needs UserId, typically from Claims in Controller, so passed in.
    public record ChangePasswordCommand(string UserId, string OldPassword, string NewPassword) : IRequest<Result<bool>>;
}
