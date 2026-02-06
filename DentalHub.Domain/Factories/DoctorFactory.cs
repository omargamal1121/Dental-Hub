using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Factories
{
    public static class DoctorFactory
    {
        public static Doctor Create(Guid userId, string specialty, int universityId)
        {
            return new Doctor
            {
                UserId = userId,
                Specialty = specialty,
                UniversityId = universityId,
                CreateAt = DateTime.UtcNow
            };
        }
    }
}