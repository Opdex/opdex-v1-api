using Opdex.Platform.Application.Abstractions.Models.MiningPools;
using Opdex.Platform.Application.Abstractions.Models.TokenDtos;

namespace Opdex.Platform.Application.Abstractions.Models.LiquidityPools
{
    public class LiquidityPoolDto
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public bool StakingEnabled { get; set; }
        public MiningPoolDto MiningPool { get; set; }
        public TokenDto SrcToken { get; set; }
        public TokenDto LpToken { get; set; }
        public TokenDto StakingToken { get; set; }
        public TokenDto CrsToken { get; set; }
        public LiquidityPoolSnapshotDto Summary { get; set; }
    }
}