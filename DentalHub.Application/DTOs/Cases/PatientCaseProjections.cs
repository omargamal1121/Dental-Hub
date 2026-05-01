using DentalHub.Application.DTOs.Cases;
using DentalHub.Application.DTOs.Diagnoses;
using DentalHub.Domain.Entities;
using Microsoft.OpenApi.Extensions;
using System.Linq.Expressions;

namespace DentalHub.Application.DTOs.Cases
{
    public static class PatientCaseProjections
    {
        public static Expression<Func<PatientCase, PatientCaseDto>> ToDto =>
            pc => new PatientCaseDto
            {
                Id = pc.Id,
                PatientId = pc.Patient.Id,
                PatientName = pc.Patient.User.FullName,
                PatientAge = pc.Patient.Age,
                Diagnoses = pc.Diagnosiss
    .Select(d => new DiagnosisDto
    {
        Id = d.Id,
        Notes = d.Notes,
        CaseTypeName = d.CaseType != null ? d.CaseType.Name : string.Empty,
        Stage = d.Stage,
        TeethNumbers = d.TeethNumbers
    })
    .ToList(),
                Status = pc.Status.ToString(),
                IsPublic = pc.IsPublic,
                UniversityId = pc.UniversityId,
                UniversityName = pc.University != null ? pc.University.Name : null,
                CreateAt = pc.CreateAt,
                TotalSessions = pc.Sessions.Count,
                PendingRequests = pc.CaseRequests.Count(cr => cr.Status == RequestStatus.Pending),
                AssignedStudentId = pc.AssignedStudentId,
                AssignedDoctorId = pc.AssignedDoctorId,
                HasEvaluatedSession = pc.Sessions.Any(s => s.Status == SessionStatus.Done),
                ImageUrls = pc.Medias.Select(m => m.MediaUrl).ToList(),
                CreatedById = pc.CreatedById,
                CreatedByRole = pc.CreatedByRole
                ,City=pc.Patient.City.GetDisplayName(),
                Phone=pc.Patient.Phone
            };
    }
}
