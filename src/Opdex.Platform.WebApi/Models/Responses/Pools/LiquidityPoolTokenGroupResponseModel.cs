namespace Opdex.Platform.WebApi.Models.Responses.Pools
{
    public class LiquidityPoolTokenGroupResponseModel
    {
        public TokenResponseModel Crs { get; set; }
        public TokenResponseModel Src { get; set; }
        public TokenResponseModel Lp { get; set; }
        public TokenResponseModel Staking { get; set; }
    }
}
