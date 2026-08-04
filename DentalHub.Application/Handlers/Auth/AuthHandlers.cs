using DentalHub.Application.Commands.Auth;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Auth;
using DentalHub.Application.Services.Auth;
using DentalHub.Application.Services.Identity;
using MediatR;

namespace DentalHub.Application.Handlers.Auth
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<TokensDto>>
    {
        private readonly IAuthenticationService _authService;

        public LoginCommandHandler(IAuthenticationService authService)
        {
            _authService = authService;
        }

        public async Task<Result<TokensDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return await _authService.LoginAsync(request.Email, request.Password);
        }
    }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<TokensDto>>
    {
        private readonly IAuthenticationService _authService;

        public RefreshTokenCommandHandler(IAuthenticationService authService)
        {
            _authService = authService;
        }

        public async Task<Result<TokensDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return await _authService.RefreshTokenAsync(request.RefreshToken);
        }
    }

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<bool>>
    {
        private readonly IAuthenticationService _authService;

        public LogoutCommandHandler(IAuthenticationService authService)
        {
            _authService = authService;
        }

        public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            return await _authService.LogoutAsync(request.UserId, request.RefreshToken);
        }
    }

    public class LogoutFromAllDevicesCommandHandler : IRequestHandler<LogoutFromAllDevicesCommand, Result<bool>>
    {
        private readonly IAuthenticationService _authService;

        public LogoutFromAllDevicesCommandHandler(IAuthenticationService authService)
        {
            _authService = authService;
        }

        public async Task<Result<bool>> Handle(LogoutFromAllDevicesCommand request, CancellationToken cancellationToken)
        {
            return await _authService.LogoutFromAllDevicesAsync(request.UserId);
        }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
    {
        private readonly IUserManagementService _userManagementService;

        public DeleteUserCommandHandler(IUserManagementService userManagementService)
        {
            _userManagementService = userManagementService;
        }

        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            return await _userManagementService.DeleteUserAsync(request.UserId);
        }
    }
}
