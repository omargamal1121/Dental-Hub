using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Factories
{
    public static class MediaFactory
    {
        public static Media Create(Guid patientId, string mediaUrl, Guid? sessionId = null)
        {
            return new Media
            {
                Id = Guid.NewGuid(),
                PatientId = patientId,
                MediaUrl = mediaUrl,
                SessionId = sessionId,
                CreateAt = DateTime.UtcNow
            };
        }
    }
}