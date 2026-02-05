using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Doctors;
using DentalHub.Application.DTOs.Doctors;

namespace DentalHub.Application.Queries.Doctor
{
    public record GetAllDoctorsQuery() : IRequest<Result<List<DoctorDto>>>;
}
