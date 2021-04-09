namespace Opdex.Platform.WebApi.Models.Responses.TransactionLogs
{
    public class TransferLogResponseModel : TransactionLogResponseModelBase
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Amount { get; set; }
    }
}