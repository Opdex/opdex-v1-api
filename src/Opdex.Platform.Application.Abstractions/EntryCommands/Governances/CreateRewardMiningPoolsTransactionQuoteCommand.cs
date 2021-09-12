using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Governances
{
    /// <summary>
    /// Create a quote for rewarding mining pools by distributing governance tokens to mine.
    /// </summary>
    public class CreateRewardMiningPoolsTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        /// <summary>
        /// Creates a reward mining pools transaction quote.
        /// </summary>
        /// <param name="governance">The governance contract address to call.</param>
        /// <param name="wallet">The wallet address public key sending the transaction.</param>
        /// <param name="fullDistribution">Flag determining if one or all nominated mining pools should be rewarded.</param>
        public CreateRewardMiningPoolsTransactionQuoteCommand(Address governance, Address wallet, bool fullDistribution) : base(wallet)
        {
            if (governance == Address.Empty)
            {
                throw new ArgumentNullException(nameof(governance), "Governance address must be provided.");
            }

            Governance = governance;
            FullDistribution = fullDistribution;
        }

        public Address Governance { get; }
        public bool FullDistribution { get; }
    }
}
