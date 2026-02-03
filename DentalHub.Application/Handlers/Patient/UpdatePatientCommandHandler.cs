using DentalHub.Application.Commands.Patient;
using DentalHub.Application.Common;
using DentalHub.Application.Services.Patients;
using DentalHub.Application.DTOs.Patients;
using MediatR;

namespace DentalHub.Application.Handlers.Patient
{
    public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, Result<bool>>
    {
        private readonly IPatientService _service;

        public UpdatePatientCommandHandler(IPatientService service)
        {
            _service = service;
        }

        public async Task<Result<bool>> Handle(UpdatePatientCommand request, CancellationToken ct)
        {
            var dto = new UpdatePatientDto
            {
                UserId = request.UserId,
                Age = request.Age,
                Phone = request.Phone
            };

            var result = await _service.UpdatePatientAsync(dto);

            if (!result.IsSuccess)
            {
                return Result<bool>.Failure(result.Message ?? "Update failed");
            }

            return Result<bool>.Success(true, "Patient updated successfully");
        }
    }
}
