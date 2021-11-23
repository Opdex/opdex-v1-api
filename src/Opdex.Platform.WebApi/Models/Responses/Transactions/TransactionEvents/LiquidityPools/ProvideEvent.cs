using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.LiquidityPools
{
    /// <summary>
    /// Liquidity providing update event.
    /// </summary>
    public abstract class ProvideEvent : TransactionEvent
    {
        /// <summary>
        /// Amount of CRS tokens transacted.
        /// </summary>
        /// <example>250.00000000</example>
        public FixedDecimal AmountCrs { get; set; }

        /// <summary>
        /// Amount of SRC tokens transacted.
        /// </summary>
        /// <example>17500.000000000000000000</example>
        public FixedDecimal AmountSrc { get; set; }

        /// <summary>
        /// Amount of LP tokens transacted.
        /// </summary>
        /// <example>25.00000000</example>
        public FixedDecimal AmountLpt { get; set; }

        /// <summary>
        /// Address of the SRC token.
        /// </summary>
        /// <example>tMdZ2UfwJorAyErDvqNdVU8kmiLaykuE5L</example>
        public Address TokenSrc { get; set; }

        /// <summary>
        /// Address of the LP token.
        /// </summary>
        /// <example>tMdZ2UfwJorAyErDvqNdVU8kmiLaykuE5L</example>
        public Address TokenLp { get; set; }

        /// <summary>
        /// Total supply of the LP token.
        /// </summary>
        /// <example>200.00000000</example>
        public FixedDecimal TokenLpTotalSupply { get; set; }
    }
}
