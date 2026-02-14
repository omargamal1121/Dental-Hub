using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;

namespace DentalHub.Application.Services.Sessions
{
    public interface ISessionService
    {
        // CRUD Operations
        Task<Result<SessionDto>> CreateSessionAsync(CreateSessionDto dto);
        Task<Result<SessionDto>> GetSessionByIdAsync(Guid sessionId);
        Task<Result<PagedResult<SessionDto>>> GetAllSessionsAsync(int page = 1, int pageSize = 10);
        Task<Result> DeleteSessionAsync(Guid sessionId);

        // Get sessions by various filters
        Task<Result<PagedResult<SessionDto>>> GetSessionsByStudentIdAsync(
            Guid studentId, int page = 1, int pageSize = 10);
        Task<Result<PagedResult<SessionDto>>> GetSessionsByPatientIdAsync(
            Guid patientId, int page = 1, int pageSize = 10);
        Task<Result<PagedResult<SessionDto>>> GetSessionsByCaseIdAsync(
            Guid caseId, int page = 1, int pageSize = 10);

        // Status management
        Task<Result<SessionDto>> UpdateSessionStatusAsync(Guid sessionId, string newStatus);

        // Session notes
        Task<Result<SessionNoteDto>> AddSessionNoteAsync(CreateSessionNoteDto dto);
        Task<Result<List<SessionNoteDto>>> GetSessionNotesAsync(Guid sessionId);
    }
}
