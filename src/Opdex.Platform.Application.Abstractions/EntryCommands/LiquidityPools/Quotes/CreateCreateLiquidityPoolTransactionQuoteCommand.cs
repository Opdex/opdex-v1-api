using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools.Quotes
{
    /// <summary>
    /// Quote a create liquidity pool transaction.
    /// </summary>
    public class CreateCreateLiquidityPoolTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        /// <summary>
        /// Creates a create liquidity pool transaction quote command.
        /// </summary>
        /// <param name="market">The address of the market.</param>
        /// <param name="wallet">The address of the wallet sending the transaction.</param>
        /// <param name="token">The address of the SRC token in the liquidity pool being created.</param>
        /// <exception cref="ArgumentException">Invalid market, wallet or token address.</exception>
        public CreateCreateLiquidityPoolTransactionQuoteCommand(Address market, Address wallet, Address token) : base(wallet)
        {
            Market = market != Address.Empty ? market : throw new ArgumentException("Market must be provided.", nameof(market));
            Token = token != Address.Empty ? token : throw new ArgumentException("Token must be provided.", nameof(token));
        }

        public Address Market { get; }
        public Address Token { get; }
    }
}
