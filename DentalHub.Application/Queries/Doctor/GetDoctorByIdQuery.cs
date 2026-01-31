using DentalHub.Application.Common.DentalHub.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Queries.Doctor
{
  public record GetDoctorByIdQuery(Guid Id)
    : IRequest<Result<DoctorDto>>;
}

