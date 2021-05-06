namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class SyncRequest : LocalWalletCredentials
    {
        public string LiquidityPool { get; set; }
    }
}