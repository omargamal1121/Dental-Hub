using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Doctors;
using MediatR;

namespace DentalHub.Application.Queries.Doctor
{
    public record GetClinicalDoctorsQuery() : IRequest<Result<List<DoctorlistDto>>>;
}
