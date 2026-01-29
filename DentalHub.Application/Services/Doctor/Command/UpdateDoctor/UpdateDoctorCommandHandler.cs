using DentalHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Services.Doctor.Command.UpdateDoctor
{
    public class UpdateDoctorCommandHandler
    {
        private readonly IMainRepository<Doctor> _repo;

        public UpdateDoctorCommandHandler(IMainRepository<Doctor repo)
        {
            _repo = repo;
        }

        public async Task Handle(UpdateDoctorCommand command)
        {
            var doctor = await _repo.GetByIdAsync(command.Id)
                ?? throw new Exception("Doctor not found");

            doctor.Update(command.Name, command.Specialization);
            await _repo.UpdateAsync(doctor);
        }
    }

}
