using DentalHub.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace DentalHub.Domain.Specifications
{
    public static class StudentSpecification
    {
        public static Expression<Func<Student, bool>> ById(Guid studentId)
        {
            return s => s.UserId == studentId;
        }
    }
}