using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.LiquidityPools
{
    public class AddLiquidityQuoteRequestModel
    {
        /// <summary>
        /// Decimal number as string of the amount of tokens to be deposited into a pool.
        /// </summary>
        [Required]
        public FixedDecimal AmountIn { get; set; }

        /// <summary>
        /// The smart contract address of the deposited token or "CRS" for Cirrus token.
        /// </summary>
        [Required]
        public Address TokenIn { get; set; }
    }
}
