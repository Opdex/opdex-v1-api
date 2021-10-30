using Opdex.Platform.Application.Abstractions.Models.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.Models.LiquidityPools
{
    public class LiquidityPoolDto
    {
        public ulong Id { get; set; }
        public string Name { get; set; }
        public Address Address { get; set; }
        public decimal TransactionFee { get; set; }
        public bool StakingEnabled { get; set; }
        public MiningPoolDto MiningPool { get; set; }
        public MarketTokenDto SrcToken { get; set; }
        public MarketTokenDto LpToken { get; set; }
        public MarketTokenDto StakingToken { get; set; }
        public TokenDto CrsToken { get; set; }
        public LiquidityPoolSnapshotDto Summary { get; set; }
    }
}
