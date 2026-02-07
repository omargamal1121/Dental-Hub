using DentalHub.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace DentalHub.Domain.Specifications
{
    public static class UserSpecification
    {
        public static Expression<Func<User, bool>> ById(Guid userId)
        {
            return u => u.Id == userId;
        }

        public static Expression<Func<User, bool>> ByEmail(string email)
        {
            return u => u.Email == email;
        }
    }
}