using DentalHub.Application.Commands.Auth;
using DentalHub.Application.DTOs.Shared;
using DentalHub.Application.Services.Auth;
using MediatR;
using DentalHub.Application.Common;

namespace DentalHub.Application.Handlers.Auth
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result<bool>>
    {
        private readonly IPasswordService _passwordService;

        public ForgotPasswordCommandHandler(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        public async Task<Result<bool>> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            return await _passwordService.RequestPasswordResetAsync(request.Email);
        }
    }

    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result<bool>>
    {
        private readonly IPasswordService _passwordService;

        public ResetPasswordCommandHandler(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        public async Task<Result<bool>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            return await _passwordService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword);
        }
    }

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result<bool>>
    {
        private readonly IPasswordService _passwordService;

        public ChangePasswordCommandHandler(IPasswordService passwordService)
        {
            _passwordService = passwordService;
        }

        public async Task<Result<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            return await _passwordService.ChangePasswordAsync(request.UserId, request.OldPassword, request.NewPassword);
        }
    }
}
