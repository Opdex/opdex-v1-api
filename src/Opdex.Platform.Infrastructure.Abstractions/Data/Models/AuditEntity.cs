using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models
{
    public abstract class AuditEntity
    {
        public DateTime CreatedDate { get; set; }
    }
}