using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Factories
{
    public static class PatientFactory
    {
        public static Patient Create(Guid userId, int age, string phone)
        {
            return new Patient
            {
                UserId = userId,
                Age = age,
                Phone = phone,
                CreateAt = DateTime.UtcNow
            };
        }
    }
}