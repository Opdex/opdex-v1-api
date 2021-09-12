using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Wallet
{
    public class ApprovedAllowanceResponseModel
    {
        public FixedDecimal Allowance { get; set; }
        public Address Owner { get; set; }
        public Address Spender { get; set; }
        public Address Token { get; set; }
    }
}
