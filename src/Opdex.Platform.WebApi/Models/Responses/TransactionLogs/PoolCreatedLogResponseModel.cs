namespace Opdex.Platform.WebApi.Models.Responses.TransactionLogs
{
    public class LiquidityPoolCreatedLogResponseModel : TransactionLogResponseModelBase
    {
        public string Token { get; set; }
        public string Pool { get; set; }
    }
}