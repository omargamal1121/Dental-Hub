using DentalHub.Domain.DomainExceptions;
using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Factories
{
    public static class CaseTypeFactory
    {
        public static CaseType Create(string name, string description)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Case type name cannot be empty");

            return new CaseType
            {
                Id = Guid.NewGuid(),
                Name = name,
                Description = description,
                CreateAt = DateTime.UtcNow
            };
        }
    }
}
