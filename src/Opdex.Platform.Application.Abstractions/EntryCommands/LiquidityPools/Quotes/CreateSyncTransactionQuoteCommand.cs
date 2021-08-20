using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools.Quotes
{
    public class CreateSyncTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        public CreateSyncTransactionQuoteCommand(Address pool, Address wallet) : base(pool, wallet)
        {
        }
    }
}
