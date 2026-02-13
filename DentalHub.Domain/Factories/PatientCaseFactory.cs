using DentalHub.Domain.DomainExceptions;
using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Factories
{
    public static class PatientCaseFactory
    {
        public static PatientCase Create(Guid patientId, Guid  casetypeid,string Description)
        {
            if (patientId == Guid.Empty)
                throw new DomainException("PatientId cannot be empty");
            if (casetypeid == Guid.Empty)
                throw new DomainException("casetypeid cannot be empty");



            return new PatientCase
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                CaseTypeId = casetypeid,
                Description = Description,


                Status = CaseStatus.Pending,
                CreateAt = DateTime.UtcNow
            };
        }
    }
}