using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;

/// <summary>
/// A base transaction quote command.
/// </summary>
public abstract class BaseTransactionQuoteCommand : IRequest<TransactionQuoteDto>
{
    /// <summary>
    /// Creates the base of a transaction quote command.
    /// </summary>
    /// <param name="walletAddress">The transaction sender's wallet address.</param>
    /// <exception cref="ArgumentException">Wallet address empty argument exception.</exception>
    protected BaseTransactionQuoteCommand(Address walletAddress)
    {
        if (walletAddress == Address.Empty)
        {
            throw new ArgumentException("Wallet address must be provided.", nameof(walletAddress));
        }

        WalletAddress = walletAddress;
    }

    public Address WalletAddress { get; }
}