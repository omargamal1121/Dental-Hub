using DentalHub.Application.Commands.CaseRequests;
using DentalHub.Application.Queries.CaseRequests;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.Services.Cases;
using MediatR;

namespace DentalHub.Application.Handlers.CaseRequests
{
    #region Commands

    public class CreateCaseRequestCommandHandler : IRequestHandler<CreateCaseRequestCommand, Result<CaseRequestDto>>
    {
        private readonly ICaseRequestService _service;
        public CreateCaseRequestCommandHandler(ICaseRequestService service) => _service = service;
        public async Task<Result<CaseRequestDto>> Handle(CreateCaseRequestCommand request, CancellationToken ct)
        {
            return await _service.CreateRequestAsync(new CreateCaseRequestDto
            {
                PatientCasePublicId = request.PatientCasePublicId,
                StudentPublicId = request.StudentPublicId,
                DoctorPublicId = request.DoctorPublicId,
                Description = request.Description
            });
        }
    }

    public class ApproveCaseRequestCommandHandler : IRequestHandler<ApproveCaseRequestCommand, Result<bool>>
    {
        private readonly ICaseRequestService _service;
        public ApproveCaseRequestCommandHandler(ICaseRequestService service) => _service = service;
        public async Task<Result<bool>> Handle(ApproveCaseRequestCommand request, CancellationToken ct)
        {
            return await _service.ApproveRequstAsync(new ApproveCaseRequestDto
            {
                RequestId = request.RequestPublicId,
                DoctorId = request.DoctorPublicId
            });
        }
    }

    public class RejectCaseRequestCommandHandler : IRequestHandler<RejectCaseRequestCommand, Result<bool>>
    {
        private readonly ICaseRequestService _service;
        public RejectCaseRequestCommandHandler(ICaseRequestService service) => _service = service;
        public async Task<Result<bool>> Handle(RejectCaseRequestCommand request, CancellationToken ct)
        {
            return await _service.RejectRequestAsync(request.RequestPublicId, request.DoctorPublicId);
        }
    }

    public class CancelCaseRequestCommandHandler : IRequestHandler<CancelCaseRequestCommand, Result>
    {
        private readonly ICaseRequestService _service;
        public CancelCaseRequestCommandHandler(ICaseRequestService service) => _service = service;
        public async Task<Result> Handle(CancelCaseRequestCommand request, CancellationToken ct)
        {
            return await _service.CancelRequestAsync(request.RequestPublicId, request.StudentPublicId);
        }
    }

    public class RejectAllRequestsForCaseCommandHandler : IRequestHandler<RejectAllRequestsForCaseCommand, Result<bool>>
    {
        private readonly ICaseRequestService _service;
        public RejectAllRequestsForCaseCommandHandler(ICaseRequestService service) => _service = service;
        public async Task<Result<bool>> Handle(RejectAllRequestsForCaseCommand request, CancellationToken ct)
        {
            return await _service.RejectAllRequestsForCaseAsync(request.CasePublicId);
        }
    }

    public class MarkAllRequestsTakenForCaseCommandHandler : IRequestHandler<MarkAllRequestsTakenForCaseCommand, Result<bool>>
    {
        private readonly ICaseRequestService _service;
        public MarkAllRequestsTakenForCaseCommandHandler(ICaseRequestService service) => _service = service;
        public async Task<Result<bool>> Handle(MarkAllRequestsTakenForCaseCommand request, CancellationToken ct)
        {
            return await _service.MarkAllRequestsTakenForCaseAsync(request.CasePublicId, request.ApprovedRequestPublicId);
        }
    }

    public class CancelAllStudentRequestsCommandHandler : IRequestHandler<CancelAllStudentRequestsCommand, Result<bool>>
    {
        private readonly ICaseRequestService _service;
        public CancelAllStudentRequestsCommandHandler(ICaseRequestService service) => _service = service;
        public async Task<Result<bool>> Handle(CancelAllStudentRequestsCommand request, CancellationToken ct)
        {
            return await _service.CancelAllStudentRequestsAsync(request.StudentPublicId);
        }
    }

    #endregion

    #region Queries

    public class GetCaseRequestByIdQueryHandler : IRequestHandler<GetCaseRequestByIdQuery, Result<CaseRequestDto>>
    {
        private readonly ICaseRequestService _service;
        public GetCaseRequestByIdQueryHandler(ICaseRequestService service) => _service = service;
        public async Task<Result<CaseRequestDto>> Handle(GetCaseRequestByIdQuery request, CancellationToken ct)
        {
            return await _service.GetRequestByPublicIdAsync(request.PublicId);
        }
    }

    public class GetCaseRequestsByDoctorIdQueryHandler : IRequestHandler<GetCaseRequestsByDoctorIdQuery, Result<PagedResult<CaseRequestDto>>>
    {
        private readonly ICaseRequestService _service;
        public GetCaseRequestsByDoctorIdQueryHandler(ICaseRequestService service) => _service = service;
        public async Task<Result<PagedResult<CaseRequestDto>>> Handle(GetCaseRequestsByDoctorIdQuery request, CancellationToken ct)
        {
            return await _service.GetRequestsByDoctorIdAsync(request.DoctorPublicId, request.Page, request.PageSize);
        }
    }

    public class GetCaseRequestsByStudentIdQueryHandler : IRequestHandler<GetCaseRequestsByStudentIdQuery, Result<PagedResult<CaseRequestDto>>>
    {
        private readonly ICaseRequestService _service;
        public GetCaseRequestsByStudentIdQueryHandler(ICaseRequestService service) => _service = service;
        public async Task<Result<PagedResult<CaseRequestDto>>> Handle(GetCaseRequestsByStudentIdQuery request, CancellationToken ct)
        {
            return await _service.GetRequestsByStudentIdAsync(request.StudentPublicId, request.Page, request.PageSize);
        }
    }

    public class GetCaseRequestsByCaseIdQueryHandler : IRequestHandler<GetCaseRequestsByCaseIdQuery, Result<IEnumerable<CaseRequestDto>>>
    {
        private readonly ICaseRequestService _service;
        public GetCaseRequestsByCaseIdQueryHandler(ICaseRequestService service) => _service = service;
        public async Task<Result<IEnumerable<CaseRequestDto>>> Handle(GetCaseRequestsByCaseIdQuery request, CancellationToken ct)
        {
            return await _service.GetRequestsByCaseIdAsync(request.CasePublicId, request.Status);
        }
    }

    #endregion
}
