using Opdex.Platform.Common.Models;

namespace Opdex.Platform.WebApi.Models.Responses.Transactions.TransactionEvents.Tokens
{
    /// <summary>
    /// Distribution event.
    /// </summary>
    public class DistributionEvent : TransactionEvent
    {
        /// <summary>
        /// Amount of governance tokens distributed to the vault.
        /// </summary>
        /// <example>75000000.00000000</example>
        public FixedDecimal VaultAmount { get; set; }

        /// <summary>
        /// Amount of governance tokens distributed to mining pools.
        /// </summary>
        /// <example>225000000.00000000</example>
        public FixedDecimal GovernanceAmount { get; set; }

        /// <summary>
        /// The distribution number.
        /// </summary>
        /// <example>1</example>
        public uint PeriodIndex { get; set; }

        /// <summary>
        /// Total amount of governance tokens distributed.
        /// </summary>
        /// <example>700000000.00000000</example>
        public FixedDecimal TotalSupply { get; set; }

        /// <summary>
        /// Block number of the next distribution.
        /// </summary>
        /// <example>750000000</example>
        public ulong NextDistributionBlock { get; set; }
    }
}
