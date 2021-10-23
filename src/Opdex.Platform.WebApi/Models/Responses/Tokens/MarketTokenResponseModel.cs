using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Tokens
{
    public class MarketTokenResponseModel
    {
        public Address Address { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public int Decimals { get; set; }
        public ulong Sats { get; set; }
        public FixedDecimal TotalSupply { get; set; }
        public TokenSummaryResponseModel Summary { get; set; }
        public Address Market { get; set; }
        public Address LiquidityPool { get; set; }
    }
}
