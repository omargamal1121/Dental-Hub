using DentalHub.Domain.Entities;
using System;

namespace DentalHub.Domain.Factories
{
    public static class MediaFactory
    {
        public static Media Create(Guid patientId, string mediaUrl, Guid? sessionId = null)
        {
            if (patientId == Guid.Empty)
                throw new ArgumentException("PatientId cannot be empty");

            if (string.IsNullOrWhiteSpace(mediaUrl))
                throw new ArgumentException("MediaUrl cannot be empty");

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