using Opdex.Platform.Common.Models;
using System;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class SkimRequest
    {
        /// <summary>
        /// The recipient of the skimmed tokens.
        /// </summary>
        [Required]
        public Address Recipient { get; set; }

        /// <summary>
        /// The liquidity pool being skimmed.
        /// </summary>
        [Obsolete] // Delete property when removing WalletBroadcast endpoints and flows.
        public Address LiquidityPool { get; set; }
    }
}
