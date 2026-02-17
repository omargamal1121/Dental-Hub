using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;

namespace DentalHub.Application.Services.Sessions
{
    public interface ISessionService
    {
        // CRUD Operations
        Task<Result<SessionDto>> CreateSessionAsync(CreateSessionDto dto);
        Task<Result<SessionDto>> GetSessionByPublicIdAsync(string publicId);
        Task<Result<PagedResult<SessionDto>>> GetAllSessionsAsync(int page = 1, int pageSize = 10);
        Task<Result> DeleteSessionByPublicIdAsync(string publicId);

        // Get sessions by various filters
        Task<Result<PagedResult<SessionDto>>> GetSessionsByStudentPublicIdAsync(
            string studentPublicId, int page = 1, int pageSize = 10);
        Task<Result<PagedResult<SessionDto>>> GetSessionsByPatientPublicIdAsync(
            string patientPublicId, int page = 1, int pageSize = 10);
        Task<Result<PagedResult<SessionDto>>> GetSessionsByCasePublicIdAsync(
            string casePublicId, int page = 1, int pageSize = 10);

        // Status management
        Task<Result<SessionDto>> UpdateSessionStatusAsync(string publicId, string newStatus);

        // Session notes
        Task<Result<SessionNoteDto>> AddSessionNoteAsync(CreateSessionNoteDto dto);
        Task<Result<List<SessionNoteDto>>> GetSessionNotesAsync(string sessionPublicId);
    }
}
