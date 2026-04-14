using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Universities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DentalHub.Application.Services.Universities
{
    public interface IUniversityService
    {
        Task<Result<IEnumerable<UniversityLookupDto>>> GetAllUniversitiesLookupAsync();
    }
}
