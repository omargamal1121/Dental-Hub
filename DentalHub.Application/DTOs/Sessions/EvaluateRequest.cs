using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.DTOs.Sessions
{
    public record EvaluateRequest(int Grade, string Note, bool IsFinalSession);
}

