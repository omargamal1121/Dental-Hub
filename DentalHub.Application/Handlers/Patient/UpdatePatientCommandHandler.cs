using DentalHub.Application.Commands.Patient;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Patients;
using DentalHub.Application.Services.Patients;
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
                FullName = request.FullName,
                Phone = request.PhoneNumber,
                NationalId = request.NationalId,
                BirthDate = request.BirthDate,
                
                Gender = request.Gender
            };

            var result = await _service.UpdatePatientAsync(dto);

            if (!result.IsSuccess)
            {
                return Result<bool>.Failure(result.Errors ?? new List<string> { result.Message ?? "Update failed" }, result.Status);
            }

            return Result<bool>.Success(true, result.Message ?? "Patient updated successfully", result.Status);
        }
    }
}
