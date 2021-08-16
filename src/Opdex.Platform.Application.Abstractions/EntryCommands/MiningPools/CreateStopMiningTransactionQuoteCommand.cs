using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools
{
    /// <summary>
    /// Quote a transaction to stop mining in a pool.
    /// </summary>
    public class CreateStopMiningTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        /// <summary>
        /// Creates a stop mining transaction quote command.
        /// </summary>
        /// <param name="miningPool">The address of the mining pool.</param>
        /// <param name="walletAddress">The transaction sender's wallet address.</param>
        /// <param name="amount">The amount to stop mining with.</param>
        /// <exception cref="ArgumentException">Invalid amount exception</exception>
        public CreateStopMiningTransactionQuoteCommand(Address miningPool, Address walletAddress, string amount)
            : base(miningPool, walletAddress)
        {
            if (!amount.IsValidDecimalNumber())
            {
                throw new ArgumentException("Amount must be a valid decimal number.", nameof(amount));
            }

            Amount = amount;
        }

        public string Amount { get; }
    }
}
