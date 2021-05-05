namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class RemoveLiquidityRequest
    {
        /// <summary>
        /// The SRC token's smart contract address
        /// </summary>
        public string Token { get; set; }
        
        /// <summary>
        /// Decimal as a string of the amount of liquidity pool tokens to remove.
        /// </summary>
        public string Liquidity { get; set; }
        
        /// <summary>
        /// Decimal as a string of the minimum amount of CRS to receive or fail the transaction.
        /// </summary>
        public string AmountCrsMin { get; set; }
        
        /// <summary>
        /// Decimal as a string of the minimum amount of SRC to receive or fail the transaction.
        /// </summary>
        public string AmountSrcMin { get; set; }
        
        /// <summary>
        /// The recipient of the CRS and SRC tokens.
        /// </summary>
        public string To { get; set; }
        
        /// <summary>
        /// The address of the market the pool belongs to.
        /// </summary>
        public string Market { get; set; }
    }
}