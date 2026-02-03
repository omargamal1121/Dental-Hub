using DentalHub.Application.Commands.Doctor;
using DentalHub.Application.Common;
using DentalHub.Application.Services.Doctors;
using MediatR;

namespace DentalHub.Application.Handlers.Doctor
{
    /// <summary>
    /// Fixed: Changed return type from Result<Guid> to Result<bool> for consistency
    /// </summary>
    public class DeleteDoctorCommandHandler : IRequestHandler<DeleteDoctorCommand, Result<bool>>
    {
        private readonly IDoctorService _service;

        public DeleteDoctorCommandHandler(IDoctorService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(DeleteDoctorCommand request, CancellationToken ct)
        {
            var result = await _service.DeleteDoctorAsync(request.Id);

            if (!result.IsSuccess)
            {
                return Result<bool>.Failure(result.Message ?? "Delete failed");
            }

            return Result<bool>.Success(true, "Doctor deleted successfully");
        }
    }
}
