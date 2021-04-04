namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class RemoveLiquidityRequest
    {
        public string Token { get; set; }
        public string Liquidity { get; set; }
        public ulong AmountCrsMin { get; set; }
        public string AmountSrcMin { get; set; }
        public string To { get; set; }
    }
}