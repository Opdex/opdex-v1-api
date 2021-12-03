using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Quotes;

/// <summary>
/// Create a token distribution transaction quote.
/// </summary>
public class CreateDistributeTokensTransactionQuoteCommand : BaseTransactionQuoteCommand
{
    /// <summary>
    /// Creates a token distribution transaction quote.
    /// </summary>
    /// <param name="token">The token contract address to call.</param>
    /// <param name="wallet">The wallet address public key sending the transaction.</param>
    public CreateDistributeTokensTransactionQuoteCommand(Address token, Address wallet) : base(wallet)
    {
        if (token == Address.Empty)
        {
            throw new ArgumentNullException(nameof(token), "Token address must be provided.");
        }

        Token = token;
    }

    public Address Token { get; }
}