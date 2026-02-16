using DentalHub.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace DentalHub.Domain.Specifications
{
    public static class DoctorSpecification
    {
        public static Expression<Func<Doctor, bool>> ByUserId(Guid userId)
        {
            return d => d.UserId == userId;
        }
    }
}