using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Doctors;
using DentalHub.Application.Queries.Doctor;
using DentalHub.Application.Specification.Comman;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.UnitOfWork;
using MediatR;

public class GetUniversityDoctorsHandler
    : IRequestHandler<GetUniversityDoctorsQuery, Result<List<DoctorLookupDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUniversityDoctorsHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<List<DoctorLookupDto>>> Handle(
        GetUniversityDoctorsQuery request,
        CancellationToken cancellationToken)
    {
        var spec = new BaseSpecification<Doctor>(
         d => d.UniversityId == request.UniversityId &&
              (string.IsNullOrEmpty(request.Name) || d.Name.Contains(request.Name))
     );

        spec.AddInclude(d => d.User);
        spec.AddInclude(d => d.University);

        var doctors = await _unitOfWork.Doctors.GetAllAsync(spec);

        if (doctors == null || !doctors.Any())
        {
            return Result<List<DoctorLookupDto>>.Success(
                new List<DoctorLookupDto>(),
                "No doctors found for this search"
            );
        }
        var result = doctors.Select(d => new DoctorLookupDto
        {
            Username = d.User.UserName,
            FullName = d.Name,
            Specialty = d.Specialty,
            UniversityName = d.University.Name
        }).ToList();

        return Result<List<DoctorLookupDto>>.Success(result);
    }
}