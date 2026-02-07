using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Doctors;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.Specification;
using DentalHub.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace DentalHub.Application.Services.Doctors
{
    public class DoctorService : IDoctorService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DoctorService> _logger;

        public DoctorService(IUnitOfWork unitOfWork, ILogger<DoctorService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region Doctor Profile

        public async Task<Result<DoctorDto>> GetDoctorByIdAsync(Guid userId)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Doctor, DoctorDto>(
                    d => d.UserId == userId,
                    d => new DoctorDto
                    {
                        UserId = d.UserId,
                        FullName = d.User.FullName,
                        Email = d.User.Email!,
                        Name = d.Name,
                        Specialty = d.Specialty,
                        UniversityId = d.UniversityId,
                        CreateAt = d.CreateAt,
                        TotalStudents = d.CaseRequests
                            .Where(cr => cr.Status == RequestStatus.Approved)
                            .Select(cr => cr.StudentId)
                            .Distinct()
                            .Count(),
                        PendingRequests = d.CaseRequests.Count(cr => cr.Status == RequestStatus.Pending),
                        ApprovedRequests = d.CaseRequests.Count(cr => cr.Status == RequestStatus.Approved)
                    }
                );

                spec.AddInclude(d => d.User);
                spec.AddInclude(d => d.CaseRequests);

                var doctor = await _unitOfWork.Doctors.GetByIdAsync(spec);

                if (doctor == null)
                {
                    return Result<DoctorDto>.Failure("Doctor not found");
                }

                return Result<DoctorDto>.Success(doctor);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor: {UserId}", userId);
                return Result<DoctorDto>.Failure("Error retrieving doctor data");
            }
        }


        public async Task<Result<List<DoctorDto>>> GetAllDoctorsAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Doctor, DoctorDto>(
                    d => new DoctorDto
                    {
                        UserId = d.UserId,
                        FullName = d.User.FullName,
                        Email = d.User.Email!,
                        Name = d.Name,
                        Specialty = d.Specialty,
                        UniversityId = d.UniversityId,
                        CreateAt = d.CreateAt,
                        TotalStudents = d.CaseRequests
                            .Where(cr => cr.Status == RequestStatus.Approved)
                            .Select(cr => cr.StudentId)
                            .Distinct()
                            .Count(),
                        PendingRequests = d.CaseRequests.Count(cr => cr.Status == RequestStatus.Pending),
                        ApprovedRequests = d.CaseRequests.Count(cr => cr.Status == RequestStatus.Approved)
                    }
                );

                spec.AddInclude(d => d.User);
                spec.AddInclude(d => d.CaseRequests);
                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderByDescending(d => d.CreateAt);

                var doctors = await _unitOfWork.Doctors.GetAllAsync(spec);

                return Result<List<DoctorDto>>.Success(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all doctors");
                return Result<List<DoctorDto>>.Failure("Error retrieving doctors");
            }
        }

   
        public async Task<Result<List<DoctorDto>>> GetDoctorsByUniversityAsync(
            int universityId, int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Doctor, DoctorDto>(
                    d => d.UniversityId == universityId,
                    d => new DoctorDto
                    {
                        UserId = d.UserId,
                        FullName = d.User.FullName,
                        Email = d.User.Email!,
                        Name = d.Name,
                        Specialty = d.Specialty,
                        UniversityId = d.UniversityId,
                        CreateAt = d.CreateAt,
                        TotalStudents = d.CaseRequests
                            .Where(cr => cr.Status == RequestStatus.Approved)
                            .Select(cr => cr.StudentId)
                            .Distinct()
                            .Count(),
                        PendingRequests = d.CaseRequests.Count(cr => cr.Status == RequestStatus.Pending),
                        ApprovedRequests = d.CaseRequests.Count(cr => cr.Status == RequestStatus.Approved)
                    }
                );

                spec.AddInclude(d => d.User);
                spec.AddInclude(d => d.CaseRequests);
                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderByDescending(d => d.CreateAt);

                var doctors = await _unitOfWork.Doctors.GetAllAsync(spec);

                return Result<List<DoctorDto>>.Success(doctors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctors by university: {UniversityId}", universityId);
                return Result<List<DoctorDto>>.Failure("Error retrieving doctors");
            }
        }


        public async Task<Result<DoctorDto>> UpdateDoctorAsync(UpdateDoctorDto dto)
        {
            try
            {
                var spec = new BaseSpecification<Doctor>(d => d.UserId == dto.UserId);
                spec.AddInclude(d => d.User);

                var doctor = await _unitOfWork.Doctors.GetByIdAsync(spec);

                if (doctor == null)
                {
                    return Result<DoctorDto>.Failure("Doctor not found");
                }

                // Update fields if provided
                if (!string.IsNullOrWhiteSpace(dto.FullName))
                {
                    doctor.User.FullName = dto.FullName;
                }

                if (!string.IsNullOrWhiteSpace(dto.Name))
                {
                    doctor.Name = dto.Name;
                }

                if (!string.IsNullOrWhiteSpace(dto.Specialty))
                {
                    doctor.Specialty = dto.Specialty;
                }

                doctor.UpdateAt = DateTime.UtcNow;

                _unitOfWork.Doctors.Update(doctor);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Doctor updated successfully: {UserId}", dto.UserId);

                return await GetDoctorByIdAsync(dto.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating doctor: {UserId}", dto.UserId);
                return Result<DoctorDto>.Failure("Error updating doctor");
            }
        }

        public async Task<Result> DeleteDoctorAsync(Guid userId)
        {
            try
            {
                var doctor = await _unitOfWork.Doctors.GetByIdAsync(
                    new BaseSpecification<Doctor>(d => d.UserId == userId));

                if (doctor == null)
                {
                    return Result.Failure("Doctor not found");
                }

                doctor.DeleteAt = DateTime.UtcNow;
                _unitOfWork.Doctors.Update(doctor);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Doctor deleted: {UserId}", userId);

                return Result.Success("Doctor deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting doctor: {UserId}", userId);
                return Result.Failure("Error deleting doctor");
            }
        }

        #endregion

        #region Statistics

        /// Get doctor statistics
        public async Task<Result<DoctorStatsDto>> GetDoctorStatisticsAsync(Guid doctorId)
        {
            try
            {
                var spec = new BaseSpecification<Doctor>(d => d.UserId == doctorId);
                spec.AddInclude(d => d.CaseRequests);

                var doctor = await _unitOfWork.Doctors.GetByIdAsync(spec);

                if (doctor == null)
                {
                    return Result<DoctorStatsDto>.Failure("Doctor not found");
                }

                // Get approved requests
                var approvedRequests = doctor.CaseRequests
                    .Where(cr => cr.Status == RequestStatus.Approved)
                    .ToList();

                // Get unique students
                var uniqueStudents = approvedRequests
                    .Select(cr => cr.StudentId)
                    .Distinct()
                    .Count();

                // Get cases from approved requests
                var caseIds = approvedRequests.Select(cr => cr.PatientCaseId).Distinct().ToList();
                
                var casesSpec = new BaseSpecification<PatientCase>(
                    pc => caseIds.Contains(pc.Id));
                var cases = await _unitOfWork.PatientCases.GetAllAsync(casesSpec);

                var stats = new DoctorStatsDto
                {
                    TotalRequests = doctor.CaseRequests.Count,
                    PendingRequests = doctor.CaseRequests.Count(cr => cr.Status == RequestStatus.Pending),
                    ApprovedRequests = approvedRequests.Count,
                    RejectedRequests = doctor.CaseRequests.Count(cr => cr.Status == RequestStatus.Rejected),
                    TotalStudents = uniqueStudents,
                    ActiveCases = cases.Count(c => c.Status == CaseStatus.InProgress),
                    CompletedCases = cases.Count(c => c.Status == CaseStatus.Completed)
                };

                return Result<DoctorStatsDto>.Success(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting doctor statistics: {DoctorId}", doctorId);
                return Result<DoctorStatsDto>.Failure("Error retrieving statistics");
            }
        }

        #endregion
    }
}
