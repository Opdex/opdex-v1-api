namespace Opdex.Platform.WebApi.Models.Responses.TransactionLogs
{
    public class ApprovalLogResponseModel : TransactionLogResponseModelBase
    {
        public string Owner { get; set; }
        public string Spender { get; set; }
        public string Amount { get; set; }
    }
}