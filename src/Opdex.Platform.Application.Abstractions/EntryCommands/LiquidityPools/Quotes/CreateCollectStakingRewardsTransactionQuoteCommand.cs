using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.LiquidityPools.Quotes
{
    public class CreateCollectStakingRewardsTransactionQuoteCommand : BaseTransactionQuoteCommand
    {
        public CreateCollectStakingRewardsTransactionQuoteCommand(Address pool, Address wallet, bool liquidate) : base(pool, wallet)
        {
            Liquidate = liquidate;
        }

        public bool Liquidate { get; }
    }
}
