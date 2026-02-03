using DentalHub.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DentalHub.Application.DTOs.Patients;

namespace DentalHub.Application.Queries.Patient
{
   public record GetPatientByIdQuery(Guid UserId)
    : IRequest<Result<PatientDto>>;
}
