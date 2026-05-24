using System;
using System.Collections.Generic;
using System.Text;

namespace BookingCare.Domain.Common
{
    public abstract class AuditableEntity : BaseEntity
    {
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; protected set; } = DateTime.UtcNow;

        protected void Touch() => UpdatedAt = DateTime.UtcNow;
    }
}
