using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class RemoveLiquidityRequest
    {
        /// <summary>
        /// Decimal as a string of the amount of liquidity pool tokens to remove.
        /// </summary>
        [Required]
        public string Liquidity { get; set; }

        /// <summary>
        /// Decimal as a string of the minimum amount of CRS to receive or fail the transaction.
        /// </summary>
        [Required]
        public string AmountCrsMin { get; set; }

        /// <summary>
        /// Decimal as a string of the minimum amount of SRC to receive or fail the transaction.
        /// </summary>
        [Required]
        public string AmountSrcMin { get; set; }

        /// <summary>
        /// The recipient of the removed liquidity.
        /// </summary>
        [Required]
        public string Recipient { get; set; }

        /// <summary>
        /// The block number limit that the transaction is valid through.
        /// </summary>
        /// <remarks>A 0 deadline is equivalent to no deadline. Anything else must be greater than the current chain height.</remarks>
        public ulong Deadline { get; set; }
    }
}
