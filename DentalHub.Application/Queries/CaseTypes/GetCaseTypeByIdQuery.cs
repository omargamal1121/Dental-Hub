using DentalHub.Application.Common;
using DentalHub.Application.DTOs.CaseTypes;
using MediatR;

namespace DentalHub.Application.Queries.CaseTypes
{
    public class GetCaseTypeByIdQuery : IRequest<Result<CaseTypeDto>>
    {
        public Guid Id { get; }

        public GetCaseTypeByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
