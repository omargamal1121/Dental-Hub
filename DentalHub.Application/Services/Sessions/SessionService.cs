using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.Specification;
using DentalHub.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace DentalHub.Application.Services.Sessions
{
    /// <summary>
    /// NEW: Complete implementation of SessionService
    /// Handles treatment sessions between students and patients
    /// </summary>
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SessionService> _logger;

        public SessionService(IUnitOfWork unitOfWork, ILogger<SessionService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region CRUD Operations

        /// <summary>
        /// Create a new session
        /// الطالب ينشئ جلسة علاج جديدة
        /// </summary>
        public async Task<Result<SessionDto>> CreateSessionAsync(CreateSessionDto dto)
        {
            try
            {
                // VALIDATION 1: Check if case exists and is InProgress
                var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(dto.CaseId);
                if (patientCase == null)
                {
                    return Result<SessionDto>.Failure("Patient case not found");
                }

                if (patientCase.Status != CaseStatus.InProgress)
                {
                    return Result<SessionDto>.Failure(
                        $"Cannot create session for case with status: {patientCase.Status}");
                }

                // VALIDATION 2: Verify patient
                if (patientCase.PatientId != dto.PatientId)
                {
                    return Result<SessionDto>.Failure("Patient ID does not match the case");
                }

                // VALIDATION 3: Check if student exists
                var student = await _unitOfWork.Students.GetByIdAsync(
                    new BaseSpecification<Student>(s => s.UserId == dto.StudentId));
                if (student == null)
                {
                    return Result<SessionDto>.Failure("Student not found");
                }

                // BUSINESS RULE 1: Verify student has approved request for this case
                var approvedRequestSpec = new BaseSpecification<CaseRequest>(cr =>
                    cr.PatientCaseId == dto.CaseId &&
                    cr.StudentId == dto.StudentId &&
                    cr.Status == RequestStatus.Approved);

                var approvedRequest = await _unitOfWork.CaseRequests.GetByIdAsync(approvedRequestSpec);
                if (approvedRequest == null)
                {
                    return Result<SessionDto>.Failure(
                        "Student does not have approval to work on this case");
                }

                // BUSINESS RULE 2: Check for overlapping sessions (same student, same time)
                var scheduledDate = dto.ScheduledAt.Date;
                var scheduledHour = dto.ScheduledAt.Hour;

                var overlappingSessionSpec = new BaseSpecification<Session>(s =>
                    s.StudentId == dto.StudentId &&
                    s.ScheduledAt.Date == scheduledDate &&
                    s.ScheduledAt.Hour == scheduledHour &&
                    s.Status != SessionStatus.Cancelled);

                var overlappingSession = await _unitOfWork.Sessions.GetByIdAsync(overlappingSessionSpec);
                if (overlappingSession != null)
                {
                    return Result<SessionDto>.Failure(
                        $"You already have a session scheduled at {dto.ScheduledAt:yyyy-MM-dd HH:mm}");
                }

                // VALIDATION 4: Cannot schedule in the past
                if (dto.ScheduledAt < DateTime.UtcNow)
                {
                    return Result<SessionDto>.Failure("Cannot schedule a session in the past");
                }

                // Create the session
                var session = new Session
                {
                    Id = Guid.NewGuid(),
                    CaseId = dto.CaseId,
                    StudentId = dto.StudentId,
                    PatientId = dto.PatientId,
                    ScheduledAt = dto.ScheduledAt,
                    Status = SessionStatus.Scheduled,
                    CreateAt = DateTime.UtcNow
                };

                await _unitOfWork.Sessions.AddAsync(session);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "Session created: {SessionId} - Student: {StudentId}, Case: {CaseId}, Scheduled: {ScheduledAt}",
                    session.Id, dto.StudentId, dto.CaseId, dto.ScheduledAt);

                return await GetSessionByIdAsync(session.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating session");
                return Result<SessionDto>.Failure("Error creating session");
            }
        }

        /// <summary>
        /// Get session by ID
        /// </summary>
        public async Task<Result<SessionDto>> GetSessionByIdAsync(Guid sessionId)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Session, SessionDto>(
                    s => s.Id == sessionId,
                    s => new SessionDto
                    {
                        Id = s.Id,
                        CaseId = s.CaseId,
                        TreatmentType = s.PatientCase.TreatmentType,
                        PatientId = s.PatientId,
                        PatientName = s.Patient.User.FullName,
                        StudentId = s.StudentId,
                        StudentName = s.Student.User.FullName,
                        ScheduledAt = s.ScheduledAt,
                        Status = s.Status.ToString(),
                        TotalNotes = s.SessionNotes.Count,
                        TotalMedia = s.Medias.Count,
                        CreateAt = s.CreateAt
                    }
                );

                spec.AddInclude("PatientCase");
                spec.AddInclude("Patient.User");
                spec.AddInclude("Student.User");
                spec.AddInclude("SessionNotes");
                spec.AddInclude("Medias");

                var session = await _unitOfWork.Sessions.GetByIdAsync(spec);

                if (session == null)
                {
                    return Result<SessionDto>.Failure("Session not found");
                }

                return Result<SessionDto>.Success(session);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting session: {SessionId}", sessionId);
                return Result<SessionDto>.Failure("Error retrieving session");
            }
        }

        /// <summary>
        /// Get all sessions with pagination
        /// </summary>
        public async Task<Result<List<SessionDto>>> GetAllSessionsAsync(int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Session, SessionDto>(
                    s => new SessionDto
                    {
                        Id = s.Id,
                        CaseId = s.CaseId,
                        TreatmentType = s.PatientCase.TreatmentType,
                        PatientId = s.PatientId,
                        PatientName = s.Patient.User.FullName,
                        StudentId = s.StudentId,
                        StudentName = s.Student.User.FullName,
                        ScheduledAt = s.ScheduledAt,
                        Status = s.Status.ToString(),
                        TotalNotes = s.SessionNotes.Count,
                        TotalMedia = s.Medias.Count,
                        CreateAt = s.CreateAt
                    }
                );

                spec.AddInclude("PatientCase");
                spec.AddInclude("Patient.User");
                spec.AddInclude("Student.User");
                spec.AddInclude("SessionNotes");
                spec.AddInclude("Medias");
                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderByDescending(s => s.ScheduledAt);

                var sessions = await _unitOfWork.Sessions.GetAllAsync(spec);

                return Result<List<SessionDto>>.Success(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all sessions");
                return Result<List<SessionDto>>.Failure("Error retrieving sessions");
            }
        }

        /// <summary>
        /// Soft delete session
        /// </summary>
        public async Task<Result> DeleteSessionAsync(Guid sessionId)
        {
            try
            {
                var session = await _unitOfWork.Sessions.GetByIdAsync(sessionId);

                if (session == null)
                {
                    return Result.Failure("Session not found");
                }

                // Can only delete scheduled sessions (not completed ones)
                if (session.Status == SessionStatus.Done)
                {
                    return Result.Failure("Cannot delete a completed session");
                }

                session.DeleteAt = DateTime.UtcNow;
                _unitOfWork.Sessions.Update(session);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Session deleted: {SessionId}", sessionId);

                return Result.Success("Session deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting session: {SessionId}", sessionId);
                return Result.Failure("Error deleting session");
            }
        }

        #endregion

        #region Filter Queries

        /// <summary>
        /// Get sessions by student ID
        /// جلسات طالب معين
        /// </summary>
        public async Task<Result<List<SessionDto>>> GetSessionsByStudentIdAsync(
            Guid studentId, int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Session, SessionDto>(
                    s => s.StudentId == studentId,
                    s => new SessionDto
                    {
                        Id = s.Id,
                        CaseId = s.CaseId,
                        TreatmentType = s.PatientCase.TreatmentType,
                        PatientId = s.PatientId,
                        PatientName = s.Patient.User.FullName,
                        StudentId = s.StudentId,
                        StudentName = s.Student.User.FullName,
                        ScheduledAt = s.ScheduledAt,
                        Status = s.Status.ToString(),
                        TotalNotes = s.SessionNotes.Count,
                        TotalMedia = s.Medias.Count,
                        CreateAt = s.CreateAt
                    }
                );

                spec.AddInclude("PatientCase");
                spec.AddInclude("Patient.User");
                spec.AddInclude("Student.User");
                spec.AddInclude("SessionNotes");
                spec.AddInclude("Medias");
                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderByDescending(s => s.ScheduledAt);

                var sessions = await _unitOfWork.Sessions.GetAllAsync(spec);

                return Result<List<SessionDto>>.Success(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sessions for student: {StudentId}", studentId);
                return Result<List<SessionDto>>.Failure("Error retrieving sessions");
            }
        }

        /// <summary>
        /// Get sessions by patient ID
        /// جلسات مريض معين
        /// </summary>
        public async Task<Result<List<SessionDto>>> GetSessionsByPatientIdAsync(
            Guid patientId, int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Session, SessionDto>(
                    s => s.PatientId == patientId,
                    s => new SessionDto
                    {
                        Id = s.Id,
                        CaseId = s.CaseId,
                        TreatmentType = s.PatientCase.TreatmentType,
                        PatientId = s.PatientId,
                        PatientName = s.Patient.User.FullName,
                        StudentId = s.StudentId,
                        StudentName = s.Student.User.FullName,
                        ScheduledAt = s.ScheduledAt,
                        Status = s.Status.ToString(),
                        TotalNotes = s.SessionNotes.Count,
                        TotalMedia = s.Medias.Count,
                        CreateAt = s.CreateAt
                    }
                );

                spec.AddInclude("PatientCase");
                spec.AddInclude("Patient.User");
                spec.AddInclude("Student.User");
                spec.AddInclude("SessionNotes");
                spec.AddInclude("Medias");
                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderByDescending(s => s.ScheduledAt);

                var sessions = await _unitOfWork.Sessions.GetAllAsync(spec);

                return Result<List<SessionDto>>.Success(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sessions for patient: {PatientId}", patientId);
                return Result<List<SessionDto>>.Failure("Error retrieving sessions");
            }
        }

        /// <summary>
        /// Get sessions by case ID
        /// كل الجلسات الخاصة بحالة معينة
        /// </summary>
        public async Task<Result<List<SessionDto>>> GetSessionsByCaseIdAsync(
            Guid caseId, int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Session, SessionDto>(
                    s => s.CaseId == caseId,
                    s => new SessionDto
                    {
                        Id = s.Id,
                        CaseId = s.CaseId,
                        TreatmentType = s.PatientCase.TreatmentType,
                        PatientId = s.PatientId,
                        PatientName = s.Patient.User.FullName,
                        StudentId = s.StudentId,
                        StudentName = s.Student.User.FullName,
                        ScheduledAt = s.ScheduledAt,
                        Status = s.Status.ToString(),
                        TotalNotes = s.SessionNotes.Count,
                        TotalMedia = s.Medias.Count,
                        CreateAt = s.CreateAt
                    }
                );

                spec.AddInclude("PatientCase");
                spec.AddInclude("Patient.User");
                spec.AddInclude("Student.User");
                spec.AddInclude("SessionNotes");
                spec.AddInclude("Medias");
                spec.ApplyPaging(page, pageSize);
                spec.ApplyOrderByDescending(s => s.ScheduledAt);

                var sessions = await _unitOfWork.Sessions.GetAllAsync(spec);

                return Result<List<SessionDto>>.Success(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sessions for case: {CaseId}", caseId);
                return Result<List<SessionDto>>.Failure("Error retrieving sessions");
            }
        }

        #endregion

        #region Status Management

        /// <summary>
        /// Update session status
        /// تغيير حالة الجلسة (Scheduled → Done / Cancelled)
        /// </summary>
        public async Task<Result<SessionDto>> UpdateSessionStatusAsync(Guid sessionId, string newStatus)
        {
            try
            {
                // Parse status
                if (!Enum.TryParse<SessionStatus>(newStatus, out var sessionStatus))
                {
                    return Result<SessionDto>.Failure("Invalid status");
                }

                var session = await _unitOfWork.Sessions.GetByIdAsync(sessionId);

                if (session == null)
                {
                    return Result<SessionDto>.Failure("Session not found");
                }

                // Validate status transition
                if (!IsValidStatusTransition(session.Status, sessionStatus))
                {
                    return Result<SessionDto>.Failure(
                        $"Cannot change status from {session.Status} to {sessionStatus}");
                }

                session.Status = sessionStatus;
                session.UpdateAt = DateTime.UtcNow;

                _unitOfWork.Sessions.Update(session);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Session status updated: {SessionId} to {Status}", sessionId, newStatus);

                return await GetSessionByIdAsync(sessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating session status: {SessionId}", sessionId);
                return Result<SessionDto>.Failure("Error updating session status");
            }
        }

        /// <summary>
        /// Validate status transition
        /// </summary>
        private bool IsValidStatusTransition(SessionStatus currentStatus, SessionStatus newStatus)
        {
            return (currentStatus, newStatus) switch
            {
                // Scheduled can go to Done or Cancelled
                (SessionStatus.Scheduled, SessionStatus.Done) => true,
                (SessionStatus.Scheduled, SessionStatus.Cancelled) => true,

                // Done and Cancelled are final states
                (SessionStatus.Done, _) => false,
                (SessionStatus.Cancelled, _) => false,

                // Same status is allowed
                _ when currentStatus == newStatus => true,

                _ => false
            };
        }

        #endregion

        #region Session Notes

        /// <summary>
        /// Add a note to a session
        /// الطالب يضيف ملاحظة للجلسة
        /// </summary>
        public async Task<Result<SessionNoteDto>> AddSessionNoteAsync(CreateSessionNoteDto dto)
        {
            try
            {
                // Verify session exists
                var session = await _unitOfWork.Sessions.GetByIdAsync(dto.SessionId);
                if (session == null)
                {
                    return Result<SessionNoteDto>.Failure("Session not found");
                }

                // Create note
                var note = new SessionNote
                {
                    Id = Guid.NewGuid(),
                    SessionId = dto.SessionId,
                    Note = dto.Note,
                    CreateAt = DateTime.UtcNow
                };

                await _unitOfWork.SessionNotes.AddAsync(note);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Session note added: {NoteId} for session {SessionId}", note.Id, dto.SessionId);

                return Result<SessionNoteDto>.Success(new SessionNoteDto
                {
                    Id = note.Id,
                    SessionId = note.SessionId,
                    Note = note.Note,
                    CreateAt = note.CreateAt
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding session note");
                return Result<SessionNoteDto>.Failure("Error adding note");
            }
        }

        /// <summary>
        /// Get all notes for a session
        /// كل الملاحظات الخاصة بجلسة معينة
        /// </summary>
        public async Task<Result<List<SessionNoteDto>>> GetSessionNotesAsync(Guid sessionId)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<SessionNote, SessionNoteDto>(
                    sn => sn.SessionId == sessionId,
                    sn => new SessionNoteDto
                    {
                        Id = sn.Id,
                        SessionId = sn.SessionId,
                        Note = sn.Note,
                        CreateAt = sn.CreateAt
                    }
                );

                spec.ApplyOrderBy(sn => sn.CreateAt);

                var notes = await _unitOfWork.SessionNotes.GetAllAsync(spec);

                return Result<List<SessionNoteDto>>.Success(notes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting session notes: {SessionId}", sessionId);
                return Result<List<SessionNoteDto>>.Failure("Error retrieving notes");
            }
        }

        #endregion
    }
}
