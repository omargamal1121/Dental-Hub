using DentalHub.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DentalHub.Domain.Interfaces
{
    internal interface IDoctorRepo
    {
        Task<Doctor?> GetByIdAsync(int id);
        Task<List<Doctor>> GetAllAsync();
        Task AddAsync(Doctor doctor);
        Task UpdateAsync(Doctor doctor);
        Task DeleteAsync(Doctor doctor);


    }
}
