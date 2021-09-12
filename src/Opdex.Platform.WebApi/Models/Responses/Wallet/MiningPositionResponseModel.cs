using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Wallet
{
    public class MiningPositionResponseModel
    {
        public Address Address { get; set; }
        public FixedDecimal Amount { get; set; }
        public Address MiningPool { get; set; }
        public Address MiningToken { get; set; }
    }
}
