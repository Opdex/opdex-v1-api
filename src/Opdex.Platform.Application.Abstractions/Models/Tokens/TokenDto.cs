using Opdex.Platform.Common.Models;
using Opdex.Platform.Common.Models.UInt;

namespace Opdex.Platform.Application.Abstractions.Models.Tokens
{
    public class TokenDto
    {
        public long Id { get; set; }
        public Address Address { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public int Decimals { get; set; }
        public ulong Sats { get; set; }
        public UInt256 TotalSupply { get; set; }
        public TokenSnapshotDto Summary { get; set; }
    }
}
