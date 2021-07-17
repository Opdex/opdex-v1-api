using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.MiningPools
{
    public class CollectMiningRewardsEventDtoAssembler : IModelAssembler<CollectMiningRewardsLog, CollectMiningRewardsEventDto>
    {
        public Task<CollectMiningRewardsEventDto> Assemble(CollectMiningRewardsLog log)
        {
            return Task.FromResult(new CollectMiningRewardsEventDto
            {
                Id = log.Id,
                TransactionId = log.TransactionId,
                SortOrder = log.SortOrder,
                Contract = log.Contract,
                EventType = TransactionEventType.CollectMiningRewardsEvent,
                Amount = log.Amount.InsertDecimal(TokenConstants.Opdex.Decimals),
                Miner = log.Miner
            });
        }
    }
}
