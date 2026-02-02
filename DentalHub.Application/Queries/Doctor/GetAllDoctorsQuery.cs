using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using DentalHub.Application.Common;
using DentalHub.Application.DTOs.Doctors;
namespace DentalHub.Application.Queries.Doctor
{
 public record GetAllDoctorsQuery()
    : IRequest<Result<List<DoctorDto>>>;
}
