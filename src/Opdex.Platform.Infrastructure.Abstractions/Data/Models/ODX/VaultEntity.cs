namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.ODX
{
    public class VaultEntity : AuditEntity
    {
        public long Id { get; set; }
        public long TokenId { get; set; }
        public string Address { get; set; }
        public string Owner { get; set; }
        public ulong Genesis { get; set; }
    }
}