//using DentalHub.Application.Commands.Doctor;
//using DentalHub.Application.Common;

//using DentalHub.Application.Services.DoctorService;
//using MediatR;
//using DentalHub.Application.Handlers;
//using DentalHub.Application.Services.DoctorService;
//namespace DentalHub.Application.Handlers.Doctor
//{
//    public class DeleteDoctorCommandHandler : IRequestHandler<DeleteDoctorCommand, Result<Guid>>



//    {
//        private readonly IDoctorService _service;

//        public DeleteDoctorCommandHandler(IDoctorService service)
//        {
//            _service = service;
//        }

//        public Task<Result<Guid>> Handle(DeleteDoctorCommand request, CancellationToken ct)

//            => _service.DeleteAsync(request.Id);
//    }
//}
