using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Factories
{
    public static class StudentFactory
    {
        public static Student Create(Guid userId, string university, int universityId, int level)
        {
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