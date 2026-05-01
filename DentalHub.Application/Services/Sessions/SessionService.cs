using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;
using DentalHub.Application.Factories;
using DentalHub.Application.Interfaces;
using DentalHub.Application.Services.Cases;
using DentalHub.Application.Specification.Comman;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.UnitOfWork;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace DentalHub.Application.Services.Sessions
{
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SessionService> _logger;
        private readonly IMediaService _mediaService;

        public SessionService(IUnitOfWork unitOfWork, ILogger<SessionService> logger, IMediaService mediaService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mediaService = mediaService;
        }

        #region CRUD Operations

        public async Task<Result<bool>> CreateSessionAsync(CreateSessionDto dto)
        {
            try
            {
                var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(new BaseSpecification<PatientCase>(pc => pc.Id == dto.CaseId));
                if (patientCase == null)
                    return Result<bool>.Failure("Patient case not found");

                if (patientCase.Status != CaseStatus.InProgress)
                    return Result<bool>.Failure($"Cannot create session for case with status: {patientCase.Status}");
                var patient = await _unitOfWork.Patients.GetByIdAsync(
                    new BaseSpecification<Patient>(p => p.Id == patientCase.PatientId));

                if (patient == null)
                    return Result<bool>.Failure("Patient not found");


                Guid doctorId;

                if (!string.IsNullOrWhiteSpace(dto.DoctorUsername))
                {
                    var doctor = await _unitOfWork.Doctors.GetByIdAsync(
                        new BaseSpecificationWithProjection<Doctor, GetIdsDto>(
                            d => d.User.UserName == dto.DoctorUsername,
                            d => new GetIdsDto { Id = d.Id }));

                    if (doctor == null)
                        return Result<bool>.Failure("Doctor not found");

                    doctorId = doctor.Id;
                }

                else
                {
                    if (patientCase.AssignedDoctorId == null)
                        return Result<bool>.Failure("This case has no assigned doctor");

                    doctorId = patientCase.AssignedDoctorId.Value;
                }

                //patientCase.AssignedDoctorId = doctorId;
                //_unitOfWork.PatientCases.Update(patientCase);

                var approvedRequest = await _unitOfWork.CaseRequests.AnyAsync(new BaseSpecification<CaseRequest>(cr =>
                    cr.PatientCaseId == patientCase.Id &&
                    cr.StudentId == dto.StudentId &&
                    cr.Status == RequestStatus.Approved));




                if (!approvedRequest)
                    return Result<bool>.Failure("Student does not have approval to work on this case");

                if (dto.ScheduledAt < DateTime.UtcNow)
                    return Result<bool>.Failure("Cannot schedule a session in the past");

                var newStart = dto.ScheduledAt;
                var newEnd = dto.ScheduledAt.AddHours(1);

                var hasOverlap = await _unitOfWork.Sessions.AnyAsync(new BaseSpecification<Session>(s =>
                    s.StudentId == dto.StudentId &&
                    s.Status == SessionStatus.Scheduled &&
                    s.StartAt < newEnd &&
                    s.EndAt > newStart
                ));

                if (hasOverlap)
                    return Result<bool>.Failure($"Student already has a session overlapping with {newStart:yyyy-MM-dd HH:mm}");
                var session = new Session
                {
                    CaseId = patientCase.Id,
                    StudentId = dto.StudentId,
                    PatientId = patientCase.PatientId,
                    EvaluteDoctorId = doctorId,
                    StartAt = dto.ScheduledAt,
                    EndAt = dto.ScheduledAt.AddHours(1),
                    Status = SessionStatus.Scheduled
                };

                await _unitOfWork.Sessions.AddAsync(session);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Session created: {Id} - Student: {StudentId}, Case: {CaseId}, Scheduled: {ScheduledAt}",
                    session.Id, dto.StudentId, dto.CaseId, dto.ScheduledAt);

                return Result<bool>.Success(true, "Session created successfully", 204);
            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating session for Student: {StudentId}, Case: {CaseId}", dto.StudentId, dto.CaseId);
                return Result<bool>.Failure("Error creating session");
            }
        }



        public async Task<Result<SessionDto>> GetSessionByIdAsync(Guid publicId)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Session, SessionDto>(
                    s => s.Id == publicId,
                    s => new SessionDto
                    {
                        Id = s.Id,
                        CaseId = s.CaseId,
                        PatientId = s.PatientId,
                        PatientName = s.Patient.User.FullName,
                        StudentId = s.StudentId,
                        StudentName = s.Student.User.FullName,
                        ScheduledAt = s.StartAt,
                        EndAt = s.EndAt,
                        Status = s.Status.ToString(),
                        TotalNotes = s.SessionNotes.Count,
                        TotalMedia = s.Medias.Count,
                        CreateAt = s.CreateAt
                    }
                );

                var session = await _unitOfWork.Sessions.GetByIdAsync(spec);
                if (session == null)
                    return Result<SessionDto>.Failure("Session not found");

                return Result<SessionDto>.Success(session);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting session: {Id}", publicId);
                return Result<SessionDto>.Failure("Error retrieving session");
            }
        }

        public async Task<Result> DeleteSessionByIdAsync(Guid publicId)
        {
            try
            {
                var session = await _unitOfWork.Sessions.GetByIdAsync(new BaseSpecification<Session>(s => s.Id == publicId));
                if (session == null)
                    return Result.Failure("Session not found");

                if (session.Status == SessionStatus.Done)
                    return Result.Failure("Cannot delete a completed session");

                session.DeleteAt = DateTime.UtcNow;
                _unitOfWork.Sessions.Update(session);
                await _unitOfWork.SaveChangesAsync();

                return Result.Success("Session deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting session: {Id}", publicId);
                return Result.Failure("Error deleting session");
            }
        }

        #endregion

        #region Filter Queries

        public async Task<Result<PagedResult<SessionDto>>> GetAllSessionsAsync(int page = 1, int pageSize = 10, string? status = null)
        {
            try
            {
                SessionStatus? sessionStatus = null;
                if (!string.IsNullOrEmpty(status) && Enum.TryParse<SessionStatus>(status, true, out var parsed))
                    sessionStatus = parsed;

                // ── projection spec (for data) ──
                var projSpec = new BaseSpecificationWithProjection<Session, SessionDto>(
                    s => !sessionStatus.HasValue || s.Status == sessionStatus.Value,
                    s => new SessionDto
                    {
                        Id = s.Id,
                        CaseId = s.CaseId,
                        PatientId = s.PatientId,
                        PatientName = s.Patient.User.FullName,
                        StudentId = s.StudentId,
                        StudentName = s.Student.User.FullName,
                        ScheduledAt = s.StartAt,
                        EndAt = s.EndAt,
                        Status = s.Status.ToString(),
                        TotalNotes = s.SessionNotes.Count,
                        TotalMedia = s.Medias.Count,
                        CreateAt = s.CreateAt
                    }
                );
                projSpec.ApplyPaging(page, pageSize);
                projSpec.ApplyOrderByDescending(s => s.StartAt);

                // ── count spec (same criteria, no paging) ──
                var countSpec = new BaseSpecification<Session>(
                    s => !sessionStatus.HasValue || s.Status == sessionStatus.Value);

                var list = await _unitOfWork.Sessions.GetAllAsync(projSpec);
                var totalCount = await _unitOfWork.Sessions.CountAsync(countSpec);

                return Result<PagedResult<SessionDto>>.Success(
                    PaginationFactory<SessionDto>.Create(totalCount, page, pageSize, list));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all sessions");
                return Result<PagedResult<SessionDto>>.Failure("Error retrieving sessions");
            }
        }

        public async Task<Result<PagedResult<SessionDto>>> GetSessionsByStudentIdAsync(Guid studentId, int page = 1, int pageSize = 10)
        {
            try
            {
                var projSpec = new BaseSpecificationWithProjection<Session, SessionDto>(
                    s => s.StudentId == studentId,
                    s => new SessionDto
                    {
                        Id = s.Id,
                        CaseId = s.CaseId,
                        PatientId = s.PatientId,
                        PatientName = s.Patient.User.FullName,
                        StudentId = s.StudentId,
                        StudentName = s.Student.User.FullName,
                        ScheduledAt = s.StartAt,
                        EndAt = s.EndAt,
                        Status = s.Status.ToString(),
                        TotalNotes = s.SessionNotes.Count,
                        TotalMedia = s.Medias.Count,
                        CreateAt = s.CreateAt
                    }
                );
                projSpec.ApplyPaging(page, pageSize);
                projSpec.ApplyOrderByDescending(s => s.StartAt);

                var countSpec = new BaseSpecification<Session>(s => s.StudentId == studentId);

                var list = await _unitOfWork.Sessions.GetAllAsync(projSpec);
                var totalCount = await _unitOfWork.Sessions.CountAsync(countSpec);

                return Result<PagedResult<SessionDto>>.Success(
                    PaginationFactory<SessionDto>.Create(totalCount, page, pageSize, list));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sessions for student: {Id}", studentId);
                return Result<PagedResult<SessionDto>>.Failure("Error retrieving sessions");
            }
        }

        public async Task<Result<PagedResult<SessionDto>>> GetSessionsByPatientIdAsync(Guid patientId, int page = 1, int pageSize = 10)
        {
            try
            {
                var projSpec = new BaseSpecificationWithProjection<Session, SessionDto>(
                    s => s.PatientId == patientId,
                    s => new SessionDto
                    {
                        Id = s.Id,
                        CaseId = s.CaseId,
                        PatientId = s.PatientId,
                        PatientName = s.Patient.User.FullName,
                        StudentId = s.StudentId,
                        StudentName = s.Student.User.FullName,
                        ScheduledAt = s.StartAt,
                        EndAt = s.EndAt,
                        Status = s.Status.ToString(),
                        TotalNotes = s.SessionNotes.Count,
                        TotalMedia = s.Medias.Count,
                        CreateAt = s.CreateAt
                    }
                );
                projSpec.ApplyPaging(page, pageSize);
                projSpec.ApplyOrderByDescending(s => s.StartAt);

                var countSpec = new BaseSpecification<Session>(s => s.PatientId == patientId);

                var list = await _unitOfWork.Sessions.GetAllAsync(projSpec);
                var totalCount = await _unitOfWork.Sessions.CountAsync(countSpec);

                return Result<PagedResult<SessionDto>>.Success(
                    PaginationFactory<SessionDto>.Create(totalCount, page, pageSize, list));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sessions for patient: {Id}", patientId);
                return Result<PagedResult<SessionDto>>.Failure("Error retrieving sessions");
            }
        }

        public async Task<Result<PagedResult<SessionDto>>> GetSessionsByCaseIdAsync(Guid caseId, int page = 1, int pageSize = 10)
        {
            try
            {
                var projSpec = new BaseSpecificationWithProjection<Session, SessionDto>(
                    s => s.CaseId == caseId,
                    s => new SessionDto
                    {
                        Id = s.Id,
                        CaseId = s.CaseId,
                        PatientId = s.PatientId,
                        PatientName = s.Patient.User.FullName,
                        StudentId = s.StudentId,
                        StudentName = s.Student.User.FullName,
                        ScheduledAt = s.StartAt,
                        EndAt = s.EndAt,
                        Status = s.Status.ToString(),
                        TotalNotes = s.SessionNotes.Count,
                        TotalMedia = s.Medias.Count,
                        CreateAt = s.CreateAt
                    }
                );
                projSpec.ApplyPaging(page, pageSize);
                projSpec.ApplyOrderByDescending(s => s.StartAt);

                var countSpec = new BaseSpecification<Session>(s => s.CaseId == caseId);

                var list = await _unitOfWork.Sessions.GetAllAsync(projSpec);
                var totalCount = await _unitOfWork.Sessions.CountAsync(countSpec);

                // Enrich each session with its notes + media
                foreach (var session in list)
                {
                    var noteSpec = new BaseSpecificationWithProjection<SessionNote, SessionNoteDto>(
                        sn => sn.SessionId == session.Id,
                        sn => new SessionNoteDto
                        {
                            Id = sn.Id,
                            SessionId = sn.SessionId,
                            Note = sn.Note,
                            CreateAt = sn.CreateAt,
                            Medias = new List<SessionMediaDto>()
                        }
                    );
                    noteSpec.ApplyOrderBy(sn => sn.CreateAt);

                    var notes = await _unitOfWork.SessionNotes.GetAllAsync(noteSpec);

                    foreach (var note in notes)
                    {
                        var mediaSpec = new BaseSpecificationWithProjection<Media, SessionMediaDto>(
                            m => m.SessionNoteId == note.Id,
                            m => new SessionMediaDto
                            {
                                Id = m.Id,
                                SessionId = session.Id,
                                NoteId = note.Id,
                                MediaUrl = m.MediaUrl,
                                CreateAt = m.CreateAt
                            }
                        );
                        note.Medias = await _unitOfWork.Medias.GetAllAsync(mediaSpec);
                    }

                    session.Notes = notes.ToList();
                    session.TotalNotes = notes.Count;
                }

                return Result<PagedResult<SessionDto>>.Success(
                    PaginationFactory<SessionDto>.Create(totalCount, page, pageSize, list));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sessions for case: {Id}", caseId);
                return Result<PagedResult<SessionDto>>.Failure("Error retrieving sessions");
            }
        }

        public async Task<Result<PagedResult<SessionDto>>> GetUpcomingSessionsAsync(
            int page = 1, int pageSize = 10,
            Guid? studentId = null, Guid? patientId = null)
        {
            try
            {
                var now = DateTime.UtcNow;

                var projSpec = new BaseSpecificationWithProjection<Session, SessionDto>(
                    s => s.Status == SessionStatus.Scheduled &&
                         s.StartAt > now &&
                         (studentId == null || s.StudentId == studentId) &&
                         (patientId == null || s.PatientId == patientId),
                    s => new SessionDto
                    {
                        Id = s.Id,
                        CaseId = s.CaseId,
                        PatientId = s.PatientId,
                        PatientName = s.Patient.User.FullName,
                        StudentId = s.StudentId,
                        StudentName = s.Student.User.FullName,
                        ScheduledAt = s.StartAt,
                        EndAt = s.EndAt,
                        Status = s.Status.ToString(),
                        TotalNotes = s.SessionNotes.Count,
                        TotalMedia = s.Medias.Count,
                        CreateAt = s.CreateAt
                    }
                );
                projSpec.ApplyPaging(page, pageSize);
                projSpec.ApplyOrderBy(s => s.StartAt); // nearest first

                var countSpec = new BaseSpecification<Session>(
                    s => s.Status == SessionStatus.Scheduled &&
                         s.StartAt > now &&
                         (studentId == null || s.StudentId == studentId) &&
                         (patientId == null || s.PatientId == patientId));

                var list = await _unitOfWork.Sessions.GetAllAsync(projSpec);
                var totalCount = await _unitOfWork.Sessions.CountAsync(countSpec);

                return Result<PagedResult<SessionDto>>.Success(
                    PaginationFactory<SessionDto>.Create(totalCount, page, pageSize, list));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting upcoming sessions");
                return Result<PagedResult<SessionDto>>.Failure("Error retrieving upcoming sessions");
            }
        }

        #endregion

        #region Status Management

        public async Task<Result<SessionDto>> UpdateSessionStatusAsync(Guid publicId, string newStatus)
        {
            try
            {
                if (!Enum.TryParse<SessionStatus>(newStatus, out var sessionStatus))
                    return Result<SessionDto>.Failure("Invalid status");

                var session = await _unitOfWork.Sessions.GetByIdAsync(new BaseSpecification<Session>(s => s.Id == publicId));
                if (session == null)
                    return Result<SessionDto>.Failure("Session not found");

                if (!IsValidStatusTransition(session.Status, sessionStatus))
                    return Result<SessionDto>.Failure($"Cannot change status from {session.Status} to {sessionStatus}");

                session.Status = sessionStatus;
                session.UpdateAt = DateTime.UtcNow;

                await _unitOfWork.SaveChangesAsync();

                return await GetSessionByIdAsync(publicId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating session status: {Id}", publicId);
                return Result<SessionDto>.Failure("Error updating session status");
            }
        }

        private static bool IsValidStatusTransition(SessionStatus current, SessionStatus next) =>
            (current, next) switch
            {
                (SessionStatus.Scheduled, SessionStatus.Done) => true,
                (SessionStatus.Scheduled, SessionStatus.Cancelled) => true,
                (SessionStatus.Done, _) => false,
                (SessionStatus.Cancelled, _) => false,
                _ when current == next => true,
                _ => false
            };

        #endregion

        #region Session Notes

        public async Task<Result<SessionNoteDto>> AddSessionNoteAsync(CreateSessionNoteDto dto)
        {
            try
            {
                var session = await _unitOfWork.Sessions.GetByIdAsync(new BaseSpecification<Session>(s => s.Id == dto.SessionId));
                if (session == null)
                    return Result<SessionNoteDto>.Failure("Session not found");

                var note = new SessionNote
                {
                    SessionId = session.Id,
                    Note = dto.Note,
                    CreateAt = DateTime.UtcNow
                };

                await _unitOfWork.SessionNotes.AddAsync(note);
                await _unitOfWork.SaveChangesAsync();

                return Result<SessionNoteDto>.Success(new SessionNoteDto
                {
                    Id = note.Id,
                    SessionId = dto.SessionId,
                    Note = note.Note,
                    CreateAt = note.CreateAt,
                    Medias = new List<SessionMediaDto>()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding session note");
                return Result<SessionNoteDto>.Failure("Error adding note");
            }
        }

        public async Task<Result<List<SessionNoteDto>>> GetSessionNotesAsync(Guid sessionPublicId)
        {
            try
            {
                // Step 1: جيبي الـ notes بدون medias
                var spec = new BaseSpecificationWithProjection<SessionNote, SessionNoteDto>(
                    sn => sn.SessionId == sessionPublicId,
                    sn => new SessionNoteDto
                    {
                        Id = sn.Id,
                        SessionId = sn.SessionId,
                        Note = sn.Note,
                        CreateAt = sn.CreateAt,
                        Medias = new List<SessionMediaDto>()
                    }
                );
                spec.ApplyOrderBy(sn => sn.CreateAt);

                var notes = await _unitOfWork.SessionNotes.GetAllAsync(spec);

                // Step 2: لكل note جيبي الـ medias في query منفصلة
                foreach (var note in notes)
                {
                    var mediaSpec = new BaseSpecificationWithProjection<Media, SessionMediaDto>(
                        m => m.SessionNoteId == note.Id,
                        m => new SessionMediaDto
                        {
                            Id = m.Id,
                            SessionId = sessionPublicId,
                            NoteId = note.Id,
                            MediaUrl = m.MediaUrl,
                            CreateAt = m.CreateAt
                        }
                    );

                    note.Medias = await _unitOfWork.Medias.GetAllAsync(mediaSpec);
                }

                return Result<List<SessionNoteDto>>.Success(notes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting session notes: {Id}", sessionPublicId);
                return Result<List<SessionNoteDto>>.Failure("Error retrieving notes");
            }
        }

        public async Task<Result<SessionMediaDto>> AddNoteMediaAsync(Guid sessionId, Guid noteId, IFormFile file)
        {
            try
            {
                var note = await _unitOfWork.SessionNotes.GetByIdAsync(
                    new BaseSpecification<SessionNote>(n => n.Id == noteId && n.SessionId == sessionId));

                if (note == null)
                    return Result<SessionMediaDto>.Failure("Note not found or does not belong to this session");

                var uploadResult = await _mediaService.SaveNoteMediaAsync(file, noteId);
                if (!uploadResult.IsSuccess)
                    return Result<SessionMediaDto>.Failure(uploadResult.Message ?? "Upload failed");

                return Result<SessionMediaDto>.Success(new SessionMediaDto
                {
                    Id = uploadResult.Data.Id,
                    SessionId = sessionId,
                    NoteId = noteId,
                    MediaUrl = uploadResult.Data.MediaUrl,
                    CreateAt = uploadResult.Data.CreateAt
                }, "Media uploaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding media to note: {NoteId}", noteId);
                return Result<SessionMediaDto>.Failure("Error uploading media");
            }
        }

        public async Task<Result<List<SessionMediaDto>>> GetNoteMediaAsync(Guid noteId)
        {
            try
            {
                var noteExists = await _unitOfWork.SessionNotes.AnyAsync(new BaseSpecification<SessionNote>(n => n.Id == noteId));
                if (!noteExists)
                    return Result<List<SessionMediaDto>>.Failure("Note not found");

                var spec = new BaseSpecificationWithProjection<Media, SessionMediaDto>(
                    m => m.SessionNoteId == noteId,
                    m => new SessionMediaDto
                    {
                        Id = m.Id,
                        SessionId = m.Session != null ? m.Session.Id : Guid.Empty,
                        NoteId = noteId,
                        MediaUrl = m.MediaUrl,
                        CreateAt = m.CreateAt
                    }
                );
                spec.ApplyOrderBy(m => m.CreateAt);

                var media = await _unitOfWork.Medias.GetAllAsync(spec);
                return Result<List<SessionMediaDto>>.Success(media);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting note media: {NoteId}", noteId);
                return Result<List<SessionMediaDto>>.Failure("Error retrieving media");
            }
        }

        public async Task<Result<SessionMediaDto>> AddSessionMediaAsync(Guid sessionId, IFormFile file)
        {
            try
            {
                var session = await _unitOfWork.Sessions.GetByIdAsync(new BaseSpecification<Session>(s => s.Id == sessionId));
                if (session == null)
                    return Result<SessionMediaDto>.Failure("Session not found");

                var uploadResult = await _mediaService.SaveSessionMediaAsync(file, sessionId);
                if (!uploadResult.IsSuccess)
                    return Result<SessionMediaDto>.Failure(uploadResult.Message ?? "Upload failed");

                var dto = new SessionMediaDto
                {
                    Id = uploadResult.Data.Id,
                    SessionId = sessionId,
                    MediaUrl = uploadResult.Data.MediaUrl,
                    CreateAt = uploadResult.Data.MediaUrl != null ? uploadResult.Data.CreateAt : DateTime.UtcNow
                };

                return Result<SessionMediaDto>.Success(dto, "Media uploaded successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding media to session: {SessionId}", sessionId);
                return Result<SessionMediaDto>.Failure("Error uploading media");
            }
        }

        public async Task<Result<List<SessionMediaDto>>> GetSessionMediaAsync(Guid sessionId)
        {
            try
            {
                var sessionExists = await _unitOfWork.Sessions.AnyAsync(new BaseSpecification<Session>(s => s.Id == sessionId));
                if (!sessionExists)
                    return Result<List<SessionMediaDto>>.Failure("Session not found");

                var spec = new BaseSpecificationWithProjection<Media, SessionMediaDto>(
                    m => m.SessionId == sessionId && m.SessionNoteId == null,
                    m => new SessionMediaDto
                    {
                        Id = m.Id,
                        SessionId = m.SessionId!.Value,
                        MediaUrl = m.MediaUrl,
                        CreateAt = m.CreateAt
                    }
                );
                spec.ApplyOrderBy(m => m.CreateAt);

                var media = await _unitOfWork.Medias.GetAllAsync(spec);
                return Result<List<SessionMediaDto>>.Success(media);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting session media: {Id}", sessionId);
                return Result<List<SessionMediaDto>>.Failure("Error retrieving media");
            }
        }

        #endregion
    }
}