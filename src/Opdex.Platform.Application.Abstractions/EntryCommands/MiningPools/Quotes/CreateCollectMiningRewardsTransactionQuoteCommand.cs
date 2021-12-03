using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools.Quotes;

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
    public CreateCollectMiningRewardsTransactionQuoteCommand(Address miningPool, Address walletAddress) : base (walletAddress)
    {
        MiningPool = miningPool != Address.Empty ? miningPool : throw new ArgumentNullException(nameof(miningPool), "Mining pool address must be set.");
    }

    public Address MiningPool { get; }
}