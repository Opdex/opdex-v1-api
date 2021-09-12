using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Wallet
{
    public class StakingPositionResponseModel
    {
        public Address Address { get; set; }
        public FixedDecimal Amount { get; set; }
        public Address LiquidityPool { get; set; }
        public Address StakingToken { get; set; }
    }
}
