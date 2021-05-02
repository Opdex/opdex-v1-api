namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens
{
    public class TokenEntity : AuditEntity
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public byte Decimals { get; set; }
        public ulong Sats { get; set; }
        public string TotalSupply { get; set; }
    }
}