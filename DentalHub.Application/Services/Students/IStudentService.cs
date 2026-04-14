using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.DTOs.Students;

namespace DentalHub.Application.Services.Students
{
    public interface IStudentService
    {
        // Student Profile
        Task<Result<StudentDetailsDto>> GetStudentByIdAsync(Guid id);

        // For the student himself - searches by UserId (coming from the JWT token)
        Task<Result<StudentDetailsDto>> GetStudentByUserIdAsync(Guid userId);

        Task<Result<PagedResult<StudentDto>>> GetAllStudentsAsync(int page = 1, int pageSize = 10, string? search = null, int? level = null);
        Task<Result<StudentDetailsDto>> UpdateStudentAsync(UpdateStudentDto dto);
        Task<Result> DeleteStudentByIdAsync(Guid id);


        Task<Result<PagedResult<PatientCaseDto>>> GetMyCasesForStudentAsync(
            Guid studentId, string? casetype = null, int page = 1, int pageSize = 10);


        Task<Result<PagedResult<AvailableCasesDto>>> GetAvailableCasesForStudentAsync(
<<<<<<< HEAD
        Guid studentId,
        string? patientName,
        string? caseType,
        Gender? gender,
        DiagnosisSource? diagnosisSource,
        CaseSortBy? sortBy,
        bool isDescending,
        int page = 1,
        int pageSize = 10);

=======
            Guid studentId, CaseFilterDto filter);
>>>>>>> 06fab53184b37dc7805f290a4acd8dead9fd536c

        // Student Statistics
        Task<Result<StudentStatsDto>> GetStudentStatisticsAsync(Guid studentId);
        Task<Result> HandleBeforeDeleteAsync(Guid id);
    }
}
