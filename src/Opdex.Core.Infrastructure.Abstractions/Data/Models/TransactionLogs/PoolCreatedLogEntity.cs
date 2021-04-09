namespace Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionLogs
{
    public class LiquidityPoolCreatedLogEntity : LogEntityBase
    {
        public string Token { get; set; }
        public string Pool { get; set; }
    }
}