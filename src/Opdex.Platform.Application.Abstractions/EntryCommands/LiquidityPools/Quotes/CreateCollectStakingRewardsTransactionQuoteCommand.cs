using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools.Quotes;

/// <summary>
/// Quote a collect staking rewards transaction.
/// </summary>
public class CreateCollectStakingRewardsTransactionQuoteCommand : BaseTransactionQuoteCommand
{
    /// <summary>
    /// Creates a collect staking rewards transaction quote command.
    /// </summary>
    /// <param name="liquidityPool">The liquidity pool address.</param>
    /// <param name="wallet">The transaction sender's wallet address.</param>
    /// <param name="liquidate">Flag indicating if rewarded OLPT tokens should be liquidated into the pool's underlying reserve tokens.</param>
    public CreateCollectStakingRewardsTransactionQuoteCommand(Address liquidityPool, Address wallet, bool liquidate) : base(wallet)
    {
        if (liquidityPool == Address.Empty)
        {
            throw new ArgumentNullException(nameof(liquidityPool), "Liquidity pool must be provided.");
        }

        LiquidityPool = liquidityPool;
        Liquidate = liquidate;
    }

    public Address LiquidityPool { get; }
    public bool Liquidate { get; }
}