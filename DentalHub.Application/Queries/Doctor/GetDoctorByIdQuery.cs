using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Doctors;

namespace DentalHub.Application.Queries.Doctor
{
    public record GetDoctorByIdQuery(Guid Id) : IRequest<Result<DoctorDto>>;
}
