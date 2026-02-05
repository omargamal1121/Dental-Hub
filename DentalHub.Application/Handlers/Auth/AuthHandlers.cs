using DentalHub.Application.Commands.Auth;
using DentalHub.Application.DTOs.Auth;
using DentalHub.Application.Services.Auth;
using MediatR;
using DentalHub.Application.Common;

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

    //public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<TokensDto>>
    //{
    //    private readonly IAuthenticationService _authService;

    //    public RefreshTokenCommandHandler(IAuthenticationService authService)
    //    {
    //        _authService = authService;
    //    }

    //    public async Task<Result<TokensDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    //    {
    //        return await _authService.RefreshTokenAsync();
    //    }
    //}

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result<bool>>
    {
        private readonly IAuthenticationService _authService;

        public LogoutCommandHandler(IAuthenticationService authService)
        {
            _authService = authService;
        }

        public async Task<Result<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            return await _authService.LogoutAsync(request.UserId);
        }
    }
}
