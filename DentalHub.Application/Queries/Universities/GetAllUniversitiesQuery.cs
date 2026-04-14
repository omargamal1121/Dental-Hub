using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Universities;
using MediatR;
using System.Collections.Generic;

namespace DentalHub.Application.Queries.Universities
{
    public class GetAllUniversitiesQuery : IRequest<Result<IEnumerable<UniversityLookupDto>>>
    {
    }
}
