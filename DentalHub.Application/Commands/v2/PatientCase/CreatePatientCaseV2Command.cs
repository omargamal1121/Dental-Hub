using MediatR;
using DentalHub.Application.Common;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using DentalHub.Domain.Entities;
using System.Text.Json.Serialization;

namespace DentalHub.Application.Commands.v2.PatientCase
{
    public class CreatePatientCaseV2Command : IRequest<Result<Guid>>
    {
        public string NationalId { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public Guid? UniversityId { get; set; }
        public List<IFormFile>? Images { get; set; }
        public InitialDiagnosisV2Dto? InitialDiagnosis { get; set; }

        // Set internally by the controller from token
        [JsonIgnore]
        public Guid? CreatedById { get; set; }
        [JsonIgnore]
        public string? CreatedByRole { get; set; }
    }

    public record InitialDiagnosisV2Dto(
        DiagnosisStage Stage,
        Guid CaseTypeId,
        string Notes,
        List<int>? TeethNumbers = null
    );
}
