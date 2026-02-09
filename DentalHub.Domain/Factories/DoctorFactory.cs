using DentalHub.Domain.DomainExceptions;
using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Factories
{
    public static class DoctorFactory
    {
        public static Doctor Create(Guid userId, string specialty, string universityId)
        {
            if (userId == Guid.Empty)
                throw new DomainException("UserId cannot be empty");

            if (string.IsNullOrWhiteSpace(specialty))
                specialty = "General";

            if (string.IsNullOrEmpty( universityId))
                throw new DomainException("UniversityId Can't Be Empty ");

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