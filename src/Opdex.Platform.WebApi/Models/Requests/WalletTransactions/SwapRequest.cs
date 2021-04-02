namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class SwapRequest
    {
        /// <summary>
        /// The address of the token being deposited. Null if CRS
        /// </summary>
        public string TokenIn { get; set; }
        
        /// <summary>
        /// The address of the token being retrieved. Null if CRS
        /// </summary>
        public string TokenOut { get; set; }
        
        /// <summary>
        /// The amount of tokens to swap
        /// </summary>
        public string TokenInAmount { get; set; }
        
        /// <summary>
        /// The amount of tokens to receive
        /// </summary>
        public string TokenOutAmount { get; set; }
        
        /// <summary>
        /// Flag either token in amount is exact or token out amount
        /// </summary>
        public bool TokenInExactAmount { get; set; }
        
        /// <summary>
        /// Slippage tolerance as a percentage
        /// </summary>
        public decimal Tolerance { get; set; }
        
        public string To { get; set; }
    }
}