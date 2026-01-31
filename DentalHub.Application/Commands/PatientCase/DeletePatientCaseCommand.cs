using DentalHub.Application.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Commands.PatientCase
{
  public record DeletePatientCaseCommand(Guid Id)
    : IRequest<Result<bool>>;

    
}

