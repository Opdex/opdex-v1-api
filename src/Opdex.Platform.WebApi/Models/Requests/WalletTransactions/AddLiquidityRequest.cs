using System;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class AddLiquidityRequest
    {
        /// <summary>
        /// Decimal as a string of the amount of CRS to deposit.
        /// </summary>
        [Required]
        public string AmountCrs { get; set; }

        /// <summary>
        /// Decimal as a string of the amount of SRC to deposit.
        /// </summary>
        [Required]
        public string AmountSrc { get; set; }

        [Required]
        public string AmountSrcMin { get; set; }

        [Required]
        public string AmountCrsMin { get; set; }

        /// <summary>
        /// Decimal number between .9999 and .0001 (99.99% to 0.01%)
        /// </summary>
        public decimal Tolerance { get; set; }

        /// <summary>
        /// The recipient of the liquidity pool tokens.
        /// </summary>
        [Required]
        public string Recipient { get; set; }

        /// <summary>
        /// The address of the liquidity pool tokens are being deposited to.
        /// </summary>
        public string LiquidityPool { get; set; }

        /// <summary>
        /// The address of the marker the pool belongs to.
        /// </summary>
        public string Market { get; set; }

        public DateTime? Deadline { get; set; }
    }
}
