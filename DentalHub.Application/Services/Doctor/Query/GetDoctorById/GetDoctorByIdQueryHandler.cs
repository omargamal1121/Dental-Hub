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

        public GetDoctorByIdQueryHandler(IMainRepository<Doctor> repo)
        {
            _repo = repo;
        }

        public async Task<DoctorDto> Handle(GetDoctorByIdQuery query)
        {
            var doctor = await _repo.GetByIdAsync(query.Id);

            if (doctor is null)
                throw new Exception("Doctor not found");

            return new DoctorDto(
                doctor.Id,
                doctor.Name,
                doctor.Specialization
            );
        }
    }

}
