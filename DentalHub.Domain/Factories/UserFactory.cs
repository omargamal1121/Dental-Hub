using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Factories
{
    public static class UserFactory
    {
        public static User Create(string fullName, string email, string password)
        {
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