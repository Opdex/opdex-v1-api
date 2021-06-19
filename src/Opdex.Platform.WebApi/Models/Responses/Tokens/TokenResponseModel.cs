using Opdex.Platform.WebApi.Models.Responses.Tokens;

namespace Opdex.Platform.WebApi.Models
{
    public class TokenResponseModel
    {
        public string Address { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public int Decimals { get; set; }
        public long Sats { get; set; }
        public string TotalSupply { get; set; }
        public TokenSummaryResponseModel Summary { get; set; }
    }
}