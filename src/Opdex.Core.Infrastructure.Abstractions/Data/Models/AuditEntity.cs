using System;

namespace Opdex.Core.Infrastructure.Abstractions.Data.Models
{
    public abstract class AuditEntity
    {
        public DateTime CreatedDate { get; set; }
    }
}