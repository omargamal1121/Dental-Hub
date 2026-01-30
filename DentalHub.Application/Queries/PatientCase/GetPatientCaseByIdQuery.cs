using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Queries.PatientCase
{
    public record GetPatientCaseByIdQuery(Guid Id)
    : IRequest<Result<PatientCaseDto>>;
}

