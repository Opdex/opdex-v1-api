using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Markets
{
    /// <summary>
    /// Quote a transaction to create a staking market.
    /// </summary>
    public class CreateCreateStakingMarketTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        /// <summary>
        /// Creates a command to quote the creation of a staking market.
        /// </summary>
        /// <param name="deployerOwner">The address of the market deployer contract owner.</param>
        /// <param name="stakingToken">The address of the token to use for staking in the markets pools.</param>
        public CreateCreateStakingMarketTransactionQuoteCommand(Address deployerOwner, Address stakingToken) : base(deployerOwner)
        {
            StakingToken = stakingToken != Address.Empty ? stakingToken : throw new ArgumentNullException(nameof(stakingToken), "Staking token address must be set.");
        }

        public Address StakingToken { get; }
    }
}
