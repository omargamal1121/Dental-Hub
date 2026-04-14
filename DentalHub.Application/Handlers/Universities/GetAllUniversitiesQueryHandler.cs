using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Universities;
using DentalHub.Application.Queries.Universities;
using DentalHub.Application.Services.Universities;
using MediatR;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DentalHub.Application.Handlers.Universities
{
    public class GetAllUniversitiesQueryHandler : IRequestHandler<GetAllUniversitiesQuery, Result<IEnumerable<UniversityLookupDto>>>
    {
        private readonly IUniversityService _service;

        public GetAllUniversitiesQueryHandler(IUniversityService service)
        {
            _service = service;
        }

        public async Task<Result<IEnumerable<UniversityLookupDto>>> Handle(GetAllUniversitiesQuery request, CancellationToken cancellationToken)
        {
            return await _service.GetAllUniversitiesLookupAsync();
        }
    }
}
