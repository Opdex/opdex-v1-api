using System.Collections.Generic;

namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public class LiquidityPoolResponseModel
    {
        public string Address { get; set; }
        public bool StakingEnabled { get; set; }
        public bool MiningEnabled { get; set; }
        public TokenResponseModel SrcToken { get; set; }
        public TokenResponseModel LpToken { get; set; }
        public TokenResponseModel StakingToken { get; set; }
        public TokenResponseModel CrsToken { get; set; }
        public LiquidityPoolSummaryResponseModel Summary { get; set; }
    }
}
