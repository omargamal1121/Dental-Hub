using DentalHub.Application.Common;
using DentalHub.Application.DTOs.CaseTypes;

namespace DentalHub.Application.Services.CaseTypes
{
    public interface ICaseTypeService
    {
        Task<Result<CaseTypeDto>> GetCaseTypeByIdAsync(Guid id);
        Task<Result<PagedResult<CaseTypeDto>>> GetAllCaseTypesAsync(int page = 1, int pageSize = 10, string? search = null);
        Task<Result<CaseTypeDto>> CreateCaseTypeAsync(CreateCaseTypeDto dto);
        Task<Result<CaseTypeDto>> UpdateCaseTypeAsync(UpdateCaseTypeDto dto);
        Task<Result> DeleteCaseTypeAsync(Guid id);
    }
}
