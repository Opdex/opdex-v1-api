using Opdex.Platform.Common.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class AddLiquidityRequest
    {
        /// <summary>
        /// Decimal amount of CRS to deposit.
        /// </summary>
        [Required]
        public FixedDecimal AmountCrs { get; set; }

        /// <summary>
        /// Decimal amount of SRC to deposit.
        /// </summary>
        [Required]
        public FixedDecimal AmountSrc { get; set; }

        /// <summary>
        /// The minimum amount of SRC tokens acceptable to provide.
        /// </summary>
        [Required]
        public FixedDecimal AmountSrcMin { get; set; }

        /// <summary>
        /// The minimum amount of CRS tokens acceptable to provide.
        /// </summary>
        [Required]
        public FixedDecimal AmountCrsMin { get; set; }

        /// <summary>
        /// Decimal number between .9999 and .0001 (99.99% to 0.01%)
        /// </summary>
        [Obsolete] // Delete property when removing WalletBroadcast endpoints and flows.
        public decimal Tolerance { get; set; }

        /// <summary>
        /// The recipient of the liquidity pool tokens.
        /// </summary>
        [Required]
        public Address Recipient { get; set; }

        /// <summary>
        /// The address of the liquidity pool tokens are being deposited to.
        /// </summary>
        [Obsolete] // Delete property when removing WalletBroadcast endpoints and flows.
        public Address LiquidityPool { get; set; }

        /// <summary>
        /// The block number limit that the transaction is valid through.
        /// </summary>
        /// <remarks>A 0 deadline is equivalent to no deadline. Anything else must be greater than the current chain height.</remarks>
        public ulong Deadline { get; set; }
    }
}
