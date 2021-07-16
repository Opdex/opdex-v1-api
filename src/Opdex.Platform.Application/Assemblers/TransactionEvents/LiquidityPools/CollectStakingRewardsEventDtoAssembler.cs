using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.LiquidityPools
{
    public class CollectStakingRewardsEventDtoAssembler : IModelAssembler<CollectStakingRewardsLog, CollectStakingRewardsEventDto>
    {
        public Task<CollectStakingRewardsEventDto> Assemble(CollectStakingRewardsLog log)
        {
            return Task.FromResult(new CollectStakingRewardsEventDto
            {
                Id = log.Id,
                TransactionId = log.TransactionId,
                SortOrder = log.SortOrder,
                Contract = log.Contract,
                LogType = log.LogType.ToString(),
                Reward = log.Reward.InsertDecimal(TokenConstants.LiquidityPoolToken.Decimals),
                Staker = log.Staker
            });
        }
    }
}
