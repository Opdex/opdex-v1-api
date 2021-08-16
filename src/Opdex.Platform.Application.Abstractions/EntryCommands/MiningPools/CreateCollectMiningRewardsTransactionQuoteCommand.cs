using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools
{
    /// <summary>
    /// Quote a transaction to collect rewards from mining in a pool.
    /// </summary>
    public class CreateCollectMiningRewardsTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        /// <summary>
        /// Creates a collect mining rewards transaction quote command.
        /// </summary>
        /// <param name="miningPool">The address of the mining pool.</param>
        /// <param name="walletAddress">The transaction sender's wallet address.</param>
        public CreateCollectMiningRewardsTransactionQuoteCommand(Address miningPool, Address walletAddress) : base(miningPool, walletAddress)
        {
        }
    }
}
