using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Quotes;

/// <summary>
/// Quote a transaction to create a standard market.
/// </summary>
public class CreateCreateStandardMarketTransactionQuoteCommand : BaseTransactionQuoteCommand
{
    /// <summary>
    /// Creates a command to quote the creation of a standard market.
    /// </summary>
    /// <param name="deployerOwner">The address of the market deployer contract owner.</param>
    /// <param name="owner">The address of the standard market owner.</param>
    /// <param name="transactionFee">Transaction fee for pools in the market, a value between 0-1%.</param>
    /// <param name="authPoolCreators">Whether to require authorization to create a pool.</param>
    /// <param name="authLiquidityProviders">Whether to require authorization to provide liquidity.</param>
    /// <param name="authTraders">Whether to require authorization to swap.</param>
    /// <param name="enableMarketFee">Whether to enable the market fee. Must be false if there is no transaction fee.</param>
    public CreateCreateStandardMarketTransactionQuoteCommand(Address deployerOwner, Address owner, decimal transactionFee, bool authPoolCreators,
                                                             bool authLiquidityProviders, bool authTraders, bool enableMarketFee) : base(deployerOwner)
    {
        Owner = owner != Address.Empty ? owner : throw new ArgumentNullException(nameof(owner), "Owner address must be set.");
        TransactionFeePercent = (transactionFee >= 0 && transactionFee <= 1) ? transactionFee : throw new ArgumentOutOfRangeException(nameof(transactionFee), "Transaction fee must be between 0 and 10.");
        AuthPoolCreators = authPoolCreators;
        AuthLiquidityProviders = authLiquidityProviders;
        AuthTraders = authTraders;
        EnableMarketFee = (transactionFee != 0 || !enableMarketFee) ? enableMarketFee : throw new ArgumentException("Market fee must be disabled if transaction fee is 0.", nameof(enableMarketFee));
    }

    public Address Owner { get; }
    public decimal TransactionFeePercent { get; }
    public bool AuthPoolCreators { get; }
    public bool AuthLiquidityProviders { get; }
    public bool AuthTraders { get; }
    public bool EnableMarketFee { get; }
}