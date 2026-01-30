using DentalHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Services.Doctor.Query.GetDoctorById
{
    public class GetDoctorByIdQueryHandler
    {
        private readonly IMainRepository<Doctor> _repo;
        public GetDoctorByIdQueryHandler(IMainRepository<Doctor>repo)
        {
            
            _repo = repo;
        }
        
    }
}
