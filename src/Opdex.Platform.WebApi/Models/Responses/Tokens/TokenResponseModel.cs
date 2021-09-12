using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Responses.Tokens;

namespace Opdex.Platform.WebApi.Models
{
    public class TokenResponseModel
    {
        public Address Address { get; set; }
        public string Name { get; set; }
        public string Symbol { get; set; }
        public int Decimals { get; set; }
        public long Sats { get; set; }
        public FixedDecimal TotalSupply { get; set; }
        public TokenSummaryResponseModel Summary { get; set; }
    }
}
