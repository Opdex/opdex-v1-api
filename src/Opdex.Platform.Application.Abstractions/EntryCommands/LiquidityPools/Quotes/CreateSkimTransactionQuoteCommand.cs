using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools.Quotes
{
    public class CreateSkimTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        public CreateSkimTransactionQuoteCommand(Address pool, Address wallet, Address recipient) : base(pool, wallet)
        {
            Recipient = recipient != Address.Empty ? recipient : throw new ArgumentNullException(nameof(recipient), "Recipient must be provided.");
        }

        public Address Recipient { get; }
    }
}
