using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Sessions;
using DentalHub.Application.Factories;
using DentalHub.Application.Services.Cases;
using DentalHub.Application.Specification.Comman;
using DentalHub.Domain.Entities;
using DentalHub.Infrastructure.UnitOfWork;
using Microsoft.Extensions.Logging;

namespace DentalHub.Application.Services.Sessions
{
    /// Handles treatment sessions between students and patients
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

        /// Student Create a new session
        public async Task<Result<SessionDto>> CreateSessionAsync(CreateSessionDto dto)
        {
            try
            {
                // VALIDATION 1: Check if case exists and is InProgress
                var patientCase = await _unitOfWork.PatientCases.GetByIdAsync(new BaseSpecification<PatientCase>(pc => pc.Id == dto.CaseId));
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
                var patient = await _unitOfWork.Patients.GetByIdAsync(new BaseSpecification<Patient>(p => p.Id == dto.PatientId));
                if (patient == null || patientCase.PatientId != patient.Id)
                {
                    return Result<SessionDto>.Failure("Patient ID does not match the case or patient not found");
                }

                // VALIDATION 3: Check if student exists
                var student = await _unitOfWork.Students.GetByIdAsync(
                    new BaseSpecification<Student>(s => s.Id     == dto.StudentId));
                if (student == null)
                {
                    return Result<SessionDto>.Failure("Student not found");
                }
                Guid doctorId;

                if (!string.IsNullOrWhiteSpace(dto.DoctorUsername))
                {
                    var doctor = await _unitOfWork.Doctors.GetByIdAsync(
                        new BaseSpecificationWithProjection<Doctor, GetIdsDto>(
                            d => d.User.UserName == dto.DoctorUsername,
                            d => new GetIdsDto { Id = d.Id }));

                    if (doctor == null)
                        return Result<SessionDto>.Failure("Doctor not found");

                    doctorId = doctor.Id;
                }

                else if (dto.DoctorId.HasValue)
                {
                    doctorId = dto.DoctorId.Value;
                }

                else
                {
                    return Result<SessionDto>.Failure("Doctor is required");
                }
                patientCase.AssignedDoctorId = doctorId;
                _unitOfWork.PatientCases.Update(patientCase);

                // BUSINESS RULE 1: Verify student has approved request for this case
                var approvedRequestSpec = new BaseSpecification<CaseRequest>(cr =>
                    cr.PatientCaseId == patientCase.Id &&
                    cr.StudentId == student.Id &&
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
                    s.StudentId == student.Id &&
                   
                    s.Status != SessionStatus.Cancelled);

                var overlappingSession = await _unitOfWork.Sessions.AnyAsync(overlappingSessionSpec);
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
                    CaseId = patientCase.Id,
                    StudentId = student.Id,
                    PatientId = patient.Id  ,
                    EvaluteDoctorId = doctorId,
                    Status = SessionStatus.Scheduled
                };

                await _unitOfWork.Sessions.AddAsync(session);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation(
                    "Session created: {PublicId} - Student: {StudentPublicId}, Case: {CasePublicId}, Scheduled: {ScheduledAt}",
                    session.Id, dto.StudentId, dto.CaseId, dto.ScheduledAt);

                return await GetSessionByIdAsync(session.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating session");
                return Result<SessionDto>.Failure("Error creating session");
            }
        }

        /// Get session by public ID
        public async Task<Result<SessionDto>> GetSessionByIdAsync(Guid publicId)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Session, SessionDto>(
                    s => s.Id == publicId,
                    s => new SessionDto
                    {
                        Id = s.Id,
                        CaseId = s.PatientCase.Id,
                      
                        PatientId = s.Patient.Id,
                        PatientName = s.Patient.User.FullName,
                        StudentId = s.Student.Id,
                        StudentName = s.Student.User.FullName,
                   
                        Status = s.Status.ToString(),
                        
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
                _logger.LogError(ex, "Error getting session by public ID: {PublicId}", publicId);
                return Result<SessionDto>.Failure("Error retrieving session");
            }
        }

        /// Get all sessions with pagination
        public async Task<Result<PagedResult<SessionDto>>> GetAllSessionsAsync(int page = 1, int pageSize = 10, string? status = null)
        {
            try
            {
                SessionStatus? sessionStatus = null;
                if (!string.IsNullOrEmpty(status))
                {
                    if (Enum.TryParse<SessionStatus>(status, true, out var parsedStatus))
                    {
                        sessionStatus = parsedStatus;
                    }
                }

                var spec = new BaseSpecificationWithProjection<Session, SessionDto>(
                    s => !sessionStatus.HasValue || s.Status == sessionStatus.Value,
                    s => new SessionDto
                    {
                        Id = s.Id,
                        CaseId = s.PatientCase.Id,
                       
                        PatientId = s.Patient.Id,
                        PatientName = s.Patient.User.FullName,
                        StudentId = s.Student.Id,
                        StudentName = s.Student.User.FullName,
                     
                        Status = s.Status.ToString(),
                      
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
             

				var sessionsList = await _unitOfWork.Sessions.GetAllAsync(spec);
				var totalCount = await _unitOfWork.Sessions.CountAsync(spec);

				var pagedResult = PaginationFactory<SessionDto>.Create(
					count: totalCount,
					page: page,
					pageSize: pageSize,
					data: sessionsList
				);

				return Result<PagedResult<SessionDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all sessions");
                return Result<PagedResult<SessionDto>>.Failure("Error retrieving sessions");
            }
        }

        /// Soft delete session
        public async Task<Result> DeleteSessionByIdAsync(Guid publicId)
        {
            try
            {
                var session = await _unitOfWork.Sessions.GetByIdAsync(new BaseSpecification<Session>(s => s.Id == publicId));

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

                _logger.LogInformation("Session deleted: {PublicId}", publicId);

                return Result.Success("Session deleted successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting session by public ID: {PublicId}", publicId);
                return Result.Failure("Error deleting session");
            }
        }

        #endregion

        #region Filter Queries

        /// Get sessions by student ID
        public async Task<Result<PagedResult<SessionDto>>> GetSessionsByStudentIdAsync(
			Guid studentPublicId, int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Session, SessionDto>(
                    s => s.Student.Id == studentPublicId,
                    s => new SessionDto
                    {
                        Id = s.Id,
                        CaseId = s.PatientCase.Id,
                     
                        PatientId = s.Patient.Id,
                        PatientName = s.Patient.User.FullName,
                        StudentId = s.Student.Id,
                        StudentName = s.Student.User.FullName,
                
                        Status = s.Status.ToString(),
                    
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
                

				var sessionsList = await _unitOfWork.Sessions.GetAllAsync(spec);
				var totalCount = await _unitOfWork.Sessions.CountAsync(spec);

				var pagedResult = PaginationFactory<SessionDto>.Create(
					count: totalCount,
					page: page,
					pageSize: pageSize,
					data: sessionsList
				);

				return Result<PagedResult<SessionDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sessions for student by public ID: {StudentPublicId}", studentPublicId);
                return Result<PagedResult<SessionDto>>.Failure("Error retrieving sessions");
            }
        }

        /// Get sessions by patient ID
        public async Task<Result<PagedResult<SessionDto>>> GetSessionsByPatientIdAsync(
			Guid patientPublicId, int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Session, SessionDto>(
                    s => s.Patient.Id == patientPublicId,
                    s => new SessionDto
                    {
                        Id = s.Id,
                        CaseId = s.PatientCase.Id,
                       
                        PatientId = s.Patient.Id,
                        PatientName = s.Patient.User.FullName,
                        StudentId = s.Student.Id,
                        StudentName = s.Student.User.FullName,
                      
                        Status = s.Status.ToString(),
                    
                        TotalMedia = s.Medias.Count,
                        CreateAt = s.CreateAt
                    }
                );

            
                spec.ApplyPaging(page, pageSize);

				var sessionsList = await _unitOfWork.Sessions.GetAllAsync(spec);
				var totalCount = await _unitOfWork.Sessions.CountAsync(spec);

				var pagedResult = PaginationFactory<SessionDto>.Create(
					count: totalCount,
					page: page,
					pageSize: pageSize,
					data: sessionsList
				);

				return Result<PagedResult<SessionDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sessions for patient by public ID: {PatientPublicId}", patientPublicId);
                return Result<PagedResult<SessionDto>>.Failure("Error retrieving sessions");
            }
        }

        /// Get sessions by case ID
        public async Task<Result<PagedResult<SessionDto>>> GetSessionsByCaseIdAsync(
			Guid casePublicId, int page = 1, int pageSize = 10)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<Session, SessionDto>(
                    s => s.PatientCase.Id == casePublicId,
                    s => new SessionDto
                    {
                        Id = s.Id,
                        CaseId = s.PatientCase.Id,
                       
                        PatientId = s.Patient.Id,
                        PatientName = s.Patient.User.FullName,
                        StudentId = s.Student.Id,
                        StudentName = s.Student.User.FullName,
                     
                        Status = s.Status.ToString(),
              
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
           
				var sessionsList = await _unitOfWork.Sessions.GetAllAsync(spec);
				var totalCount = await _unitOfWork.Sessions.CountAsync(spec);

				var pagedResult = PaginationFactory<SessionDto>.Create(
					count: totalCount,
					page: page,
					pageSize: pageSize,
					data: sessionsList
				);

				return Result<PagedResult<SessionDto>>.Success(pagedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting sessions for case by public ID: {CasePublicId}", casePublicId);
                return Result<PagedResult<SessionDto>>.Failure("Error retrieving sessions");
            }
        }

        #endregion

        #region Status Management

        /// Update session status (Scheduled → Done / Cancelled)
        public async Task<Result<SessionDto>> UpdateSessionStatusAsync(Guid publicId, string newStatus)
        {
            try
            {
                // Parse status
                if (!Enum.TryParse<SessionStatus>(newStatus, out var sessionStatus))
                {
                    return Result<SessionDto>.Failure("Invalid status");
                }

                var session = await _unitOfWork.Sessions.GetByIdAsync(new BaseSpecification<Session>(s => s.Id == publicId));

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

                _logger.LogInformation("Session status updated: {PublicId} to {Status}", publicId, newStatus);

                return await GetSessionByIdAsync(publicId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating session status by public ID: {PublicId}", publicId);
                return Result<SessionDto>.Failure("Error updating session status");
            }
        }

        /// Validate status transition
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

        /// Student Add a note to a session
        public async Task<Result<SessionNoteDto>> AddSessionNoteAsync(CreateSessionNoteDto dto)
        {
            try
            {
                // Verify session exists
                var session = await _unitOfWork.Sessions.GetByIdAsync(new BaseSpecification<Session>(s => s.Id == dto.SessionId));
                if (session == null)
                {
                    return Result<SessionNoteDto>.Failure("Session not found");
                }

                // Create note
                var note = new SessionNote
                {
                    SessionId = session.Id,
                    Note = dto.Note,
                    CreateAt = DateTime.UtcNow
                };

                await _unitOfWork.SessionNotes.AddAsync(note);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Session note added: {PublicId} for session {SessionPublicId}", note.Id, dto.SessionId);

                return Result<SessionNoteDto>.Success(new SessionNoteDto
                {
                    Id = note.Id,
                    SessionId = dto.SessionId,
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

        /// Get all notes for a session
        public async Task<Result<List<SessionNoteDto>>> GetSessionNotesAsync(Guid sessionPublicId)
        {
            try
            {
                var spec = new BaseSpecificationWithProjection<SessionNote, SessionNoteDto>(
                    sn => sn.Session.Id == sessionPublicId,
                    sn => new SessionNoteDto
                    {
                        Id = sn.Id,
                        SessionId = sn.Session.Id,
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
                _logger.LogError(ex, "Error getting session notes for session by public ID: {SessionPublicId}", sessionPublicId);
                return Result<List<SessionNoteDto>>.Failure("Error retrieving notes");
            }
        }

        #endregion
    }
}
