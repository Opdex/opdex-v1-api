using NJsonSchema.Annotations;
using Opdex.Platform.Common.Models;
using Opdex.Platform.WebApi.Models.Responses.LiquidityPools.Summary;

namespace Opdex.Platform.WebApi.Models.Responses.LiquidityPools
{
    public class LiquidityPoolResponseModel
    {
        /// <summary>
        /// The contract address of the liquidity pool.
        /// </summary>
        [NotNull]
        public Address Address { get; set; }

        /// <summary>
        /// The name of the liquidity pool.
        /// </summary>
        [NotNull]
        public string Name { get; set; }

        /// <summary>
        /// The transaction fee for swaps.
        /// </summary>
        [NotNull]
        public decimal TransactionFee { get; set; }

        /// <summary>
        /// The primary token's involved in the pool.
        /// </summary>
        [NotNull]
        public LiquidityPoolTokenGroupResponseModel Token { get; set; }

        /// <summary>
        /// The pool's current sumamry.
        /// </summary>
        [NotNull]
        public LiquidityPoolSummaryResponseModel Summary { get; set; }
    }
}
