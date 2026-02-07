using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Factories
{
    public static class DoctorFactory
    {
        public static Doctor Create(Guid userId, string specialty, int universityId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty");

            if (string.IsNullOrWhiteSpace(specialty))
                specialty = "General";

            if (universityId <= 0)
                throw new ArgumentException("UniversityId must be greater than 0");

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