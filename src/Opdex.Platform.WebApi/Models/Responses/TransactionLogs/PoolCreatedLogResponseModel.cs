namespace Opdex.Platform.WebApi.Models.Responses.TransactionLogs
{
    public class CreateLiquidityPoolLogResponseModel : TransactionLogResponseModelBase
    {
        public string Token { get; set; }
        public string LiquidityPool { get; set; }
    }
}