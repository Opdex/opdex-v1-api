using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools
{
    /// <summary>
    /// Quote a transaction to start mining in a pool.
    /// </summary>
    public class CreateStartMiningTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        /// <summary>
        /// Creates a stop mining transaction quote command.
        /// </summary>
        /// <param name="miningPool">The address of the mining pool.</param>
        /// <param name="walletAddress">The transaction sender's wallet address.</param>
        /// <param name="amount">The amount to start mining with.</param>
        /// <exception cref="ArgumentException">Invalid amount exception</exception>
        public CreateStartMiningTransactionQuoteCommand(Address miningPool, Address walletAddress, FixedDecimal amount)
            : base(walletAddress)
        {
            MiningPool = miningPool != Address.Empty ? miningPool : throw new ArgumentNullException(nameof(miningPool), "Mining pool address must be set.");
            Amount = amount > FixedDecimal.Zero ? amount : throw new ArgumentOutOfRangeException(nameof(amount), "Amount must be greater than 0.");
        }

        public Address MiningPool { get; }
        public FixedDecimal Amount { get; }
    }
}
