namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class AddLiquidityRequest
    {
        public string Token { get; set; }
        
        /// <summary>
        /// Amount of CRS in statoshis (e.g 1 crs = 100000000)
        /// </summary>
        public ulong AmountCrsDesired { get; set; }
        public string AmountSrcDesired { get; set; }
        public ulong AmountCrsMin { get; set; }
        public string AmountSrcMin { get; set; }
        public string To { get; set; }
        public string Market { get; set; }
    }
}