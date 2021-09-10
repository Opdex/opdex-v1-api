using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Markets
{
    public class CreateLiquidityPoolEventResponseModel : TransactionEventResponseModel
    {
        public Address Token { get; set; }
        public Address LiquidityPool { get; set; }
    }
}
