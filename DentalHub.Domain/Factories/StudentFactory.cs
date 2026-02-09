using DentalHub.Domain.DomainExceptions;
using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Factories
{
    public static class StudentFactory
    {
        public static Student Create(Guid userId, string university, string universityId)
        {
            if (userId == Guid.Empty)
                throw new DomainException("UserId cannot be empty");

            if (string.IsNullOrWhiteSpace(university))
                university = "Unknown University";

            if (string.IsNullOrEmpty(universityId))
                throw new DomainException("UniversityId Can't Be Empty");

           
            return new Student
            {
                UserId = userId,
                University = university,
                UniversityId = universityId,
                Level = 1,
                CreateAt = DateTime.UtcNow
            };
        }
    }
}