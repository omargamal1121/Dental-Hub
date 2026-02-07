using DentalHub.Domain.Entities;
using System;
using System.Text.RegularExpressions;

namespace DentalHub.Domain.Factories
{
    public static class UserFactory
    {
        public static User Create(string fullName, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("FullName cannot be empty");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty");

            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            if (!emailRegex.IsMatch(email))
                throw new ArgumentException("Email format is invalid");

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                throw new ArgumentException("Password must be at least 6 characters");

            return new User
            {
                Id = Guid.CreateVersion7(),
                FullName = fullName,
                UserName = email,
                Email = email,
                PasswordHash = password,
            };
        }
    }
}