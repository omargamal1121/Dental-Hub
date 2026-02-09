using DentalHub.Domain.DomainExceptions;
using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Factories
{
    public static class StudentFactory
    {
        public static Student Create(Guid userId, string university, int universityId, int level)
        {
            if (userId == Guid.Empty)
                throw new DomainException("UserId cannot be empty");

            if (string.IsNullOrWhiteSpace(university))
                university = "Unknown University";

            if (universityId <= 0)
                throw new DomainException("UniversityId must be greater than 0");

            if (level <= 0 || level > 6)
                throw new DomainException("Level must be between 1 and 6");

            return new Student
            {
                UserId = userId,
                University = university,
                UniversityId = universityId,
                Level = level,
                CreateAt = DateTime.UtcNow
            };
        }
    }
}