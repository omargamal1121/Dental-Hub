using DentalHub.Application.Commands.Doctor;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Services.Cases;
using MediatR;

namespace DentalHub.Application.Handlers.Doctor
{
    public class ApproveMyRequestCommandHandler : IRequestHandler<ApproveMyRequestCommand, Result<bool>>
    {
        private readonly ICaseRequestService _caseRequestService;

        public ApproveMyRequestCommandHandler(ICaseRequestService caseRequestService)
        {
            _caseRequestService = caseRequestService;
        }

        public async Task<Result<bool>> Handle(ApproveMyRequestCommand request, CancellationToken cancellationToken)
        {
            var dto = new ApproveCaseRequestDto
            {
                RequestId = request.RequestId,
                DoctorId = request.DoctorUserId,
                IsApproved = true
            };

            var result = await _caseRequestService.ApproveRequstAsync(dto);

            if (!result.IsSuccess)
                return Result<bool>.Failure(result.Errors ?? new List<string> { result.Message ?? "Failed to approve" }, result.Status);

            return Result<bool>.Success(true, "Request approved successfully", result.Status);
        }
    }

    public class RejectMyRequestCommandHandler : IRequestHandler<RejectMyRequestCommand, Result<bool>>
    {
        private readonly ICaseRequestService _caseRequestService;

        public RejectMyRequestCommandHandler(ICaseRequestService caseRequestService)
        {
            _caseRequestService = caseRequestService;
        }

        public async Task<Result<bool>> Handle(RejectMyRequestCommand request, CancellationToken cancellationToken)
        {
            var result = await _caseRequestService.RejectRequestAsync(
                request.RequestId,
                request.DoctorUserId);

            if (!result.IsSuccess)
                return Result<bool>.Failure(result.Errors ?? new List<string> { result.Message ?? "Failed to reject" }, result.Status);

            return Result<bool>.Success(true, "Request rejected successfully", result.Status);
        }
    }
}
