using DentalHub.Domain.Entities;
using System;
using System.Linq.Expressions;

namespace DentalHub.Domain.Specifications
{
    public static class SessionNoteSpecification
    {
        public static Expression<Func<SessionNote, bool>> BySessionId(Guid sessionId)
        {
            return sn => sn.SessionId == sessionId;
        }
    }
}