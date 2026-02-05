using DentalHub.Application.Commands.Doctor;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Doctors;
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
            var dto = new UpdateDoctorDto
            {
                UserId = request.Id,
                Name = request.Name,
                Specialty = request.Specialty,
                UniversityId = request.UniversityId,
               
                
            };

            var result = await _service.UpdateDoctorAsync(dto);

            if (!result.IsSuccess)
            {
                return Result<bool>.Failure(result.Errors ?? new List<string> { result.Message ?? "Update failed" }, result.Status);
            }

            return Result<bool>.Success(true, result.Message ?? "Doctor updated successfully", result.Status);
        }
    }
}
