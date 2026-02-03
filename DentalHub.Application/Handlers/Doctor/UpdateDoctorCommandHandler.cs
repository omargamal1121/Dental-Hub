using DentalHub.Application.Commands.Doctor;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Doctors;
using DentalHub.Application.Services;
using DentalHub.Application.Services.Doctors;
using MediatR;

namespace DentalHub.Application.Handlers.Doctor
{
    public class UpdateDoctorCommandHandler : IRequestHandler<UpdateDoctorCommand, Result<bool>>
    {
        private readonly IDoctorService _service;

        public UpdateDoctorCommandHandler(IDoctorService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(UpdateDoctorCommand request, CancellationToken ct)
        {
            // Command TO DTO
            var dto = new UpdateDoctorDto
            {
                UserId = request.Id,
                Name = request.Name,
                Specialty = request.Specialty
            };

            var result = await _service.UpdateDoctorAsync(dto);

            if (!result.IsSuccess)
            {
                return Result<bool>.Failure(result.Message ?? "Update failed");
            }

            return Result<bool>.Success(true, "Doctor updated successfully");
        }
    }
}
