using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools.Quotes;

/// <summary>
/// Quote a remove liquidity transaction.
/// </summary>
public class CreateRemoveLiquidityTransactionQuoteCommand : BaseTransactionQuoteCommand
{
    /// <summary>
    /// Creates a remove liquidity transaction quote command.
    /// </summary>
    /// <param name="liquidityPool">The address of the liquidity pool.</param>
    /// <param name="wallet">The address of the wallet sending the transaction.</param>
    /// <param name="amountLpt">The amount of liquidity pool tokens to burn and remove liquidity for.</param>
    /// <param name="amountCrsMin">The minimum amount of CRS tokens to collect.</param>
    /// <param name="amountSrcMin">The minimum amount of STC tokens to collect.</param>
    /// <param name="recipient">The recipient of the returned liquidity pool tokens.</param>
    /// <param name="deadline">The block deadline that the transaction is valid before.</param>
    /// <exception cref="ArgumentException">Invalid liquidity pool, amounts, or recipient command parameters.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Invalid deadline</exception>
    public CreateRemoveLiquidityTransactionQuoteCommand(Address liquidityPool, Address wallet, FixedDecimal amountLpt, FixedDecimal amountCrsMin,
                                                        FixedDecimal amountSrcMin, Address recipient, ulong deadline) : base(wallet)
    {
        if (liquidityPool == Address.Empty)
        {
            throw new ArgumentNullException(nameof(liquidityPool), "Liquidity pool must be provided.");
        }

        if (recipient == Address.Empty)
        {
            throw new ArgumentNullException(nameof(recipient), "Recipient must be provided.");
        }

        if (amountLpt <= FixedDecimal.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(amountLpt), "Amount LPT burned must be greater than 0.");
        }

        if (amountCrsMin <= FixedDecimal.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(amountCrsMin), "Amount CRS minimum must be greater than 0.");
        }

        if (amountSrcMin <= FixedDecimal.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(amountSrcMin), "Amount SRC minimum must be greater than 0.");
        }

        LiquidityPool = liquidityPool;
        AmountLpt = amountLpt;
        AmountCrsMin = amountCrsMin;
        AmountSrcMin = amountSrcMin;
        Recipient = recipient;
        Deadline = deadline;
    }

    public Address LiquidityPool { get; }
    public FixedDecimal AmountLpt { get; }
    public FixedDecimal AmountCrsMin { get; }
    public FixedDecimal AmountSrcMin { get; }
    public Address Recipient { get; }
    public ulong Deadline { get; }
}