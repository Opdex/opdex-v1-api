using Opdex.Platform.Application.Abstractions.EntryCommands.Transactions;
using Opdex.Platform.Common.Models;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.MiningPools
{
    public class CreateCollectMiningRewardsTransactionQuoteCommand : BaseQuoteCommand
    {
        public CreateCollectMiningRewardsTransactionQuoteCommand(Address miningPool, Address walletAddress)
            : base(miningPool, walletAddress)
        {
        }
    }
}
