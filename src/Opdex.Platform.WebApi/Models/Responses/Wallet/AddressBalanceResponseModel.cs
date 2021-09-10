using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Wallet
{
    public class AddressBalanceResponseModel
    {
        public FixedDecimal Balance { get; set; }

        public Address Address { get; set; }

        public Address Token { get; set; }
    }
}
