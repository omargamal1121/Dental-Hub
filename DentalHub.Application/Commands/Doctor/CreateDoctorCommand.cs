using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
namespace DentalHub.Application.Commands.Doctor
{
    public record CreateDoctorCommand(
    string Name,
    Guid UserId,
    string Specialty,
    int UniversityId
) : IRequest<Result<Guid>>;

}
