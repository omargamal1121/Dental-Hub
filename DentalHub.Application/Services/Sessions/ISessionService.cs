using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;
using Microsoft.AspNetCore.Http;

namespace DentalHub.Application.Services.Sessions
{
    public interface ISessionService
    {
        // CRUD
        Task<Result<bool>> CreateSessionAsync(CreateSessionDto dto);
        Task<Result<SessionDto>> GetSessionByIdAsync(Guid id);
        Task<Result<PagedResult<SessionDto>>> GetAllSessionsAsync(int page = 1, int pageSize = 10, string? status = null);
        Task<Result> DeleteSessionByIdAsync(Guid id);

        // Filters
        Task<Result<PagedResult<SessionDto>>> GetSessionsByStudentIdAsync(Guid studentId, int page = 1, int pageSize = 10);
        Task<Result<PagedResult<SessionDto>>> GetSessionsByPatientIdAsync(Guid patientId, int page = 1, int pageSize = 10);
        Task<Result<PagedResult<SessionDto>>> GetSessionsByCaseIdAsync(Guid caseId, int page = 1, int pageSize = 10);
        Task<Result<PagedResult<SessionDto>>> GetUpcomingSessionsAsync(int page = 1, int pageSize = 10, Guid? studentId = null, Guid? patientId = null);

        // Status
        Task<Result<SessionDto>> UpdateSessionStatusAsync(Guid id, string newStatus);

        // Notes
        Task<Result<SessionNoteDto>> AddSessionNoteAsync(CreateSessionNoteDto dto);
        Task<Result<List<SessionNoteDto>>> GetSessionNotesAsync(Guid sessionId);

        // Session-level media
        Task<Result<SessionMediaDto>> AddSessionMediaAsync(Guid sessionId, IFormFile file);
        Task<Result<List<SessionMediaDto>>> GetSessionMediaAsync(Guid sessionId);

        // Note-level media
        Task<Result<SessionMediaDto>> AddNoteMediaAsync(Guid sessionId, Guid noteId, IFormFile file);
        Task<Result<List<SessionMediaDto>>> GetNoteMediaAsync(Guid noteId);
        Task<Result<Guid>> EvaluateSessionAsync(Guid sessionId, Guid doctorId, int grade, string note, bool isFinalSession);

    }
}
