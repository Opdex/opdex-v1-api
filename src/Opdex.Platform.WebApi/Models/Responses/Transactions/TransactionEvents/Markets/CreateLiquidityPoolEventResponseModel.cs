namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Markets
{
    public class CreateLiquidityPoolEventResponseModel : TransactionEventResponseModel
    {
        public string Token { get; set; }
        public string LiquidityPool { get; set; }
    }
}
