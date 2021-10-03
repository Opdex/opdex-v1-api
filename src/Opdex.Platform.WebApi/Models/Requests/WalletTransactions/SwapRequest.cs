using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class SwapRequest
    {
        /// <summary>
        /// The address of the token being retrieved. Null if CRS
        /// </summary>
        [Required]
        public Address TokenOut { get; set; }

        /// <summary>
        /// The amount of tokens to swap
        /// </summary>
        [Required]
        public FixedDecimal TokenInAmount { get; set; }

        /// <summary>
        /// The amount of tokens to receive
        /// </summary>
        [Required]
        public FixedDecimal TokenOutAmount { get; set; }

        /// <summary>
        /// Flag either token in amount is exact or token out amount
        /// </summary>
        [Required]
        public bool TokenInExactAmount { get; set; }

        /// <summary>
        /// The maximum amount of tokens willing to swap
        /// </summary>
        [Required]
        public FixedDecimal TokenInMaximumAmount { get; set; }

        /// <summary>
        /// The minimum amount of tokens acceptable to receive
        /// </summary>
        [Required]
        public FixedDecimal TokenOutMinimumAmount { get; set; }

        /// <summary>
        /// The recipient of the swapped tokens.
        /// </summary>
        [Required]
        public Address Recipient { get; set; }

        /// <summary>
        /// The deadline the transaction should complete by or fail.
        /// </summary>
        public ulong Deadline { get; set; }
    }
}
