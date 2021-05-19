namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses
{
    public class AddressMiningEntity : AuditEntity
    {
        public long Id { get; set; }
        public long MiningPoolId { get; set; }
        public string Owner { get; set; }
        public string Balance { get; set; }
    }
}