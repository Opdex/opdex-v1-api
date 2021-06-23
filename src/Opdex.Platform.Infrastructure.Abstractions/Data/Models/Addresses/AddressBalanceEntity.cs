namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses
{
    public class AddressBalanceEntity : AuditEntity
    {
        public long Id { get; set; }
        public long TokenId { get; set; }
        public string Owner { get; set; }
        public string Balance { get; set; }
    }
}