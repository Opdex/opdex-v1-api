namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models
{
    public class DeployerEntity : AuditEntity
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public string Owner { get; set; }
    }
}