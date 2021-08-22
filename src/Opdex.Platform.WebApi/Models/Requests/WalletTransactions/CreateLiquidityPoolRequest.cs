using Opdex.Platform.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Opdex.Platform.WebApi.Models.Requests.WalletTransactions
{
    public class CreateLiquidityPoolRequest
    {
        /// <summary>
        /// The address of the SRC token to create a liquidity pool for.
        /// </summary>
        [Required]
        public Address Token { get; set; }
    }
}
