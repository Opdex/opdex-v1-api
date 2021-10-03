using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models
{
    public class DeployerEntity : AuditEntity
    {
        public long Id { get; set; }
        public Address Address { get; set; }
        public Address PendingOwner { get; set; }
        public Address Owner { get; set; }
        public bool IsActive { get; set; }
    }
}
