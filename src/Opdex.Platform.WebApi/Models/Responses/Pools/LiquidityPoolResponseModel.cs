namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public class LiquidityPoolResponseModel
    {
        public string Address { get; set; }
        public bool StakingEnabled { get; set; }
        public bool MiningEnabled { get; set; }
        public TokenResponseModel Token { get; set; }
        public LiquidityPoolSummaryResponseModel Summary { get; set; }
    }
}