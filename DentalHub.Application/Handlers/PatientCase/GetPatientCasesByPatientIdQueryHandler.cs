//using DentalHub.Application.Common;
//using DentalHub.Application.DTOs.Cases;
//using DentalHub.Application.Queries.PatientCase;
//using DentalHub.Application.Services.Cases;
//using MediatR;

//namespace DentalHub.Application.Handlers.PatientCase
//{
//    public class GetPatientCasesByPatientIdQueryHandler : IRequestHandler<GetPatientCasesByPatientIdQuery, Result<List<PatientCaseDto>>>
//    {
//        private readonly IPatientCaseService _service;

//        public GetPatientCasesByPatientIdQueryHandler(IPatientCaseService service)
//        {
//            _service = service;
//        }

//        public async Task<Result<List<PatientCaseDto>>> Handle(GetPatientCasesByPatientIdQuery request, CancellationToken ct)
//        {
//            return await _service.(request.PatientId);
//        }
//    }
//}
