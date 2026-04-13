using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Doctors;
using DentalHub.Application.DTOs.Shared;
using MediatR;

namespace DentalHub.Application.Queries.Doctor
{
    public record GetUniversityDoctorsQuery(Guid UniversityId, string? Name)
        : IRequest<Result<List<DoctorLookupDto>>>;
}