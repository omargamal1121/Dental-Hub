using DentalHub.Domain.DomainExceptions;
using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Factories
{
    public static class StudentFactory
    {
        public static Student Create(Guid userId, string university, Guid universityId)
        {
            if (userId == Guid.Empty)
                throw new DomainException("UserId cannot be empty");

            if (string.IsNullOrWhiteSpace(university))
                university = "Unknown University";

            if (universityId == Guid.Empty)
                throw new DomainException("UniversityId can't be empty");

            return new Student
            {
              
                UniversityId = universityId,
                Level = 1,
                CreateAt = DateTime.UtcNow
            };
    }
    }
}