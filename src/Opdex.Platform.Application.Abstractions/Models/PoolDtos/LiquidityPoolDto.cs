using Opdex.Platform.Application.Abstractions.Models.TokenDtos;

namespace Opdex.Platform.Application.Abstractions.Models.PoolDtos
{
    public class LiquidityPoolDto
    {
        public long Id { get; set; }
        public string Address { get; set; }
        public bool StakingEnabled { get; set; }
        public bool MiningEnabled { get; set; }
        public TokenDto SrcToken { get; set; }
        public TokenDto LpToken { get; set; }
        public TokenDto StakingToken { get; set; }
        public TokenDto CrsToken { get; set; }
        public LiquidityPoolSnapshotDto Summary { get; set; }
    }
}
