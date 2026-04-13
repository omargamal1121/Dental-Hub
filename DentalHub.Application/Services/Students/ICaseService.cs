using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Cases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Services.Students
{
    public interface ICaseService
    {
        Task<PagedResult<AvailableCasesDto>> GetAvailableCasesAsync(
            Guid studentId,
            string? patientName,
            string? caseType,
            Gender? gender,
            DiagnosisSource? diagnosisSource,
            CaseSortBy? sortBy,
            bool isDescending,
            int page,
            int pageSize);
    }

}
