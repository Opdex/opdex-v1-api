using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.MiningGovernances;

/// <summary>
/// Create a quote for rewarding mining pools by distributing miningGovernance tokens to mine.
/// </summary>
public class CreateRewardMiningPoolsTransactionQuoteCommand : BaseTransactionQuoteCommand
{
    /// <summary>
    /// Creates a reward mining pools transaction quote.
    /// </summary>
    /// <param name="miningGovernance">The miningGovernance contract address to call.</param>
    /// <param name="wallet">The wallet address public key sending the transaction.</param>
    /// <param name="fullDistribution">Flag determining if one or all nominated mining pools should be rewarded.</param>
    public CreateRewardMiningPoolsTransactionQuoteCommand(Address miningGovernance, Address wallet, bool fullDistribution) : base(wallet)
    {
        if (miningGovernance == Address.Empty)
        {
            throw new ArgumentNullException(nameof(miningGovernance), "Mining governance address must be provided.");
        }

        MiningGovernance = miningGovernance;
        FullDistribution = fullDistribution;
    }

    public Address MiningGovernance { get; }
    public bool FullDistribution { get; }
}