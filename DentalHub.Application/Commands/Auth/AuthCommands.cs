using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Auth;
using DentalHub.Application.DTOs.Shared;
using MediatR;

namespace DentalHub.Application.Commands.Auth
{
    public record LoginCommand(string Email, string Password) : IRequest<Result<TokensDto>>;
    public record RefreshTokenCommand() : IRequest<Result<TokensDto>>;
    public record LogoutCommand(Guid UserId) : IRequest<Result<bool>>;
    public record DeleteUserCommand(Guid UserId) : IRequest<Result>;
}
