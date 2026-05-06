using DentalHub.Application.DTOs.CaseTypes;
using DentalHub.Application.DTOs.Diagnoses;
using DentalHub.Domain.Entities;

namespace DentalHub.Application.DTOs.Cases
{
    /// DTO for patient case information
    public class PatientCaseDto
    {
        public Guid Id { get; set; }
        public string Description { get; set; } = string.Empty;

        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int PatientAge { get; set; }

        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Process status - lifecycle of the case
        /// AIPreliminaryDiagnosis | DiagnosedInClinic | UnAssigned | InProgress | Evaluated | Completed
        /// </summary>
        public string ProcessStatus { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string NationalId { get; set; } = string.Empty;

        public bool IsPublic { get; set; }

        public Guid? UniversityId { get; set; }
        public string? UniversityName { get; set; }

        public DateTime CreateAt { get; set; }

        public int TotalSessions { get; set; }
        public bool HasEvaluatedSession { get; set; }
        public int PendingRequests { get; set; }

        public Guid? AssignedStudentId { get; set; }
        public Guid? AssignedDoctorId { get; set; }

        // ✅ FIXED: بدل Diagnosisdto المفردة
        public List<DiagnosisDto> Diagnosisdto { get; set; } = new();

        public List<string> ImageUrls { get; set; } = new();

        public Guid? CreatedById { get; set; }
        public string? CreatedByRole { get; set; }

        /// <summary>
        /// Flags describing the current user's relationship to this case
        /// </summary>
        public CaseUserFlags UserFlags { get; set; } = new();

        /// <summary>
        /// Available actions for current user
        /// </summary>
        public List<string> AvailableActions { get; set; } = new();
    }


 

    /// <summary>
    /// Flags describing the relationship between the current user and the case
    /// </summary>
    public class CaseUserFlags
    {
        /// <summary>The current user is the patient who owns this case</summary>
        public bool IsOwner { get; set; }

		/// <summary>The current user is a Doctor</summary>
		public string Role { get; set; }

		/// <summary>The current user is the Doctor assigned to supervise this case</summary>
		public bool IsAssignedDoctor { get; set; }

        /// <summary>The case is assigned to a Student</summary>
        public bool IsAssignedStudent { get; set; }

        /// <summary>The case is assigned to the current user (Doctor or Student)</summary>
        public bool IsAssignedToMe { get; set; }

        /// <summary>The current user has a pending request for this case</summary>
        public bool HasRequest { get; set; }
        public Guid? RequestId { get; set; }
        public string? RequestStatus { get; set; }

     
    }
}
