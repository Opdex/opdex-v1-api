namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models
{
    public abstract class AuditEntity
    {
        public ulong CreatedBlock { get; set; }
        public ulong ModifiedBlock { get; set; }
    }
}
