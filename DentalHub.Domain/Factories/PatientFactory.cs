using DentalHub.Domain.DomainExceptions;
using DentalHub.Domain.Entities;
using System;
using System.Text.RegularExpressions;

namespace DentalHub.Domain.Factories
{
    public static class PatientFactory
    {
        public static Patient Create(Guid userId, int age, string phone)
        {
            if (userId == Guid.Empty)
                throw new DomainException("UserId cannot be empty");

            if (age <= 0)
                throw new DomainException("Age must be greater than 0");

            if (string.IsNullOrWhiteSpace(phone))
                throw new DomainException("Phone cannot be empty");

            var phoneRegex = new Regex(@"^\+?\d{7,15}$");
            if (!phoneRegex.IsMatch(phone))
                throw new DomainException("Phone format is invalid");

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