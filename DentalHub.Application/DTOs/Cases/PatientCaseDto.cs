using DentalHub.Application.DTOs.CaseTypes;
using DentalHub.Domain.Entities;

namespace DentalHub.Application.DTOs.Cases
{
    /// DTO for patient case information
    public class PatientCaseDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public int PatientAge { get; set; }
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Process status - clearer picture of the case lifecycle:
        /// AIPreliminaryDiagnosis | DiagnosedInClinic | UnAssigned | InProgress | Evaluated | Completed
        /// </summary>
        public string ProcessStatus { get; set; } = string.Empty;

        public bool IsPublic { get; set; }
        public Guid? UniversityId { get; set; }
        public string? UniversityName { get; set; }
        public DateTime CreateAt { get; set; }
        public int TotalSessions { get; set; }
        public bool HasEvaluatedSession { get; set; }
        public int PendingRequests { get; set; }
        public Guid? AssignedStudentId { get; set; }
        public Guid? AssignedDoctorId { get; set; }
        public Diagnosisdto? Diagnosisdto { get; set; }
        public List<string> ImageUrls { get; set; } = new List<string>();

        /// <summary>
        /// Flags describing the current user's relationship to this case
        /// </summary>
        public CaseUserFlags UserFlags { get; set; } = new CaseUserFlags();

        /// <summary>
        /// Actions available to the current user based on their relationship and the case state
        /// </summary>
        public List<string> AvailableActions { get; set; } = new List<string>();
    }

    public class Diagnosisdto
    {
        public Guid Id { get; set; }
        public string DiagnosisStage { get; set; }
        public string CaseType { get; set; }
        public string Notes { get; set; }
        public List<int> TeethNumbers { get; set; } = new List<int>();
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
