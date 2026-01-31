using DentalHub.Application.Commands.Doctor;
using DentalHub.Application.Common;
using DentalHub.Application.Common.DentalHub.Domain.Common;
using DentalHub.Application.Services.Doctor;
using MediatR;

namespace DentalHub.Application.Handlers.Doctor
{
    public class DeleteDoctorCommandHandler : IRequestHandler<DeleteDoctorCommand, Result>
    {
        private readonly IDoctorService _service;

        public DeleteDoctorCommandHandler(IDoctorService service)
        {
            _service = service;
        }

        public Task<Result> Handle(DeleteDoctorCommand request, CancellationToken ct)
            => _service.DeleteAsync(request.Id);
    }
}
