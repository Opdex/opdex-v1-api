namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class CreatePoolRequest
    {
        public string Token { get; set; }
        public string Sender { get; set; }
        public string Market { get; set; }
    }
}