using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools.Quotes
{
    public class CreateCreateLiquidityPoolTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        public CreateCreateLiquidityPoolTransactionQuoteCommand(Address market, Address wallet, Address token) : base(market, wallet)
        {
            Token = token != Address.Empty ? token : throw new ArgumentNullException(nameof(token), "Token must be provided.");
        }

        public Address Token { get; }
    }
}
