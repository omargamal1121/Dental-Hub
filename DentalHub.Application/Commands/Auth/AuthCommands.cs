using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Auth;
using DentalHub.Application.DTOs.Shared;
using MediatR;

namespace DentalHub.Application.Commands.Auth
{
  
    public record LoginCommand(string Email, string Password) : IRequest<Result<TokensDto>>;
    public record LoginCommandWithIp(string Email, string Password, string IpAdress) : IRequest<Result<TokensDto>>;

    public record RefreshTokenCommand(string RefreshToken) : IRequest<Result<TokensDto>>;

    public record LogoutCommand(Guid UserId,string RefreshToken) : IRequest<Result<bool>>;

 
    public record LogoutFromAllDevicesCommand(Guid UserId) : IRequest<Result<bool>>;

 
    public record DeleteUserCommand(Guid UserId) : IRequest<Result>;
}
