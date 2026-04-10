using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Diagnoses;
using MediatR;

namespace DentalHub.Application.Queries.Diagnoses
{
    public class GetDiagnosisByIdQuery : IRequest<Result<DiagnosisDto>>
    {
        public Guid Id { get; set; }

        public GetDiagnosisByIdQuery(Guid id)
        {
            Id = id;
        }
    }
}
