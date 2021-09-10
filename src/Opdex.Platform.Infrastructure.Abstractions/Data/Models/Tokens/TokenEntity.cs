using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Models.Tokens
{
    public class TokenEntity : AuditEntity
    {
        public long Id { get; set; }
        public bool IsLpt { get; set; }
        public string Address { get; set; }
        public string Symbol { get; set; }
        public string Name { get; set; }
        public byte Decimals { get; set; }
        public ulong Sats { get; set; }
        public UInt256 TotalSupply { get; set; }
    }
}
