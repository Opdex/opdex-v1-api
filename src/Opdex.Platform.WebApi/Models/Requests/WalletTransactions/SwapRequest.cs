using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    /// <summary>
    /// A request to quote a token swap.
    /// </summary>
    public class SwapRequest
    {
        /// <summary>
        /// The address of the token being retrieved.
        /// </summary>
        /// <example>CRS</example>
        [Required]
        public Address TokenOut { get; set; }

        /// <summary>
        /// The amount of tokens to swap.
        /// </summary>
        /// <example>"50.00000000"</example>
        [Required]
        public FixedDecimal TokenInAmount { get; set; }

        /// <summary>
        /// The amount of tokens to receive
        /// </summary>
        /// <example>"5.00000000"</example>
        [Required]
        public FixedDecimal TokenOutAmount { get; set; }

        /// <summary>
        /// Flag either token in amount is exact or token out amount
        /// </summary>
        /// <example>true</example>
        [Required]
        public bool TokenInExactAmount { get; set; }

        /// <summary>
        /// The maximum amount of tokens willing to swap
        /// </summary>
        /// <example>"50.00000000"</example>
        [Required]
        public FixedDecimal TokenInMaximumAmount { get; set; }

        /// <summary>
        /// The minimum amount of tokens acceptable to receive
        /// </summary>
        /// <example>"4.95000000"</example>
        [Required]
        public FixedDecimal TokenOutMinimumAmount { get; set; }

        /// <summary>
        /// The recipient of the swapped tokens.
        /// </summary>
        /// <example>tQ9RukZsB6bBsenHnGSo1q69CJzWGnxohm</example>
        [Required]
        public Address Recipient { get; set; }

        /// <summary>
        /// The deadline the transaction should complete by or fail.
        /// </summary>
        /// <example>500000</example>
        public ulong Deadline { get; set; }
    }
}
