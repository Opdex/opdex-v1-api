namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class AddLiquidityRequest
    {
        public string Token { get; set; }
        public ulong AmountCrsDesired { get; set; }
        public string AmountSrcDesired { get; set; }
        public ulong AmountCrsMin { get; set; }
        public string AmountSrcMin { get; set; }
        public string To { get; set; }
    }
}