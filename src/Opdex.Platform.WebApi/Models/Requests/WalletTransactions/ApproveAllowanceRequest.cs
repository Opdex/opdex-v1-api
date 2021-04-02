namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class ApproveAllowanceRequest
    {
        public string Token { get; set; }
        public string Amount { get; set; }
        public string Owner { get; set; }
        public string Spender { get; set; }
    }
}