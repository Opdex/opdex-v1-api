namespace Opdex.Core.Infrastructure.Abstractions.Data.Models.TransactionLogs
{
    public class TransferLogEntity : LogEntityBase
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Amount { get; set; }
    }
}