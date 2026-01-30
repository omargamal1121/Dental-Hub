using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Application.Services.Doctor.Command.DeleteDoctor
{
    public class DeleteDoctorCommandHandler
    {
        private readonly IMainRepository<Doctor> _repo;

        public DeleteDoctorCommandHandler(IMainRepository<Doctor> repo)
        {
            _repo = repo;
        }

        public async Task Handle(DeleteDoctorCommand command)
        {
            var doctor = await _repo.GetByIdAsync(command.Id)
                ?? throw new Exception("Doctor not found");

            await _repo.DeleteAsync(doctor);
        }
    }

}
