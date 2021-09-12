using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Governances
{
    /// <summary>
    ///
    /// </summary>
    public class CreateRewardMiningPoolsTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="governance"></param>
        /// <param name="wallet"></param>
        /// <param name="fullDistribution"></param>
        /// <exception cref="ArgumentNullException"></exception>
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
