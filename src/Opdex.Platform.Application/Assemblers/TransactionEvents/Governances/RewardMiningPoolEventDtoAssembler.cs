using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Governances;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.Governances;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.Governances
{
    public class RewardMiningPoolEventDtoAssembler : IModelAssembler<RewardMiningPoolLog, RewardMiningPoolEventDto>
    {
        public Task<RewardMiningPoolEventDto> Assemble(RewardMiningPoolLog log)
        {
            return Task.FromResult(new RewardMiningPoolEventDto
            {
                Id = log.Id,
                TransactionId = log.TransactionId,
                SortOrder = log.SortOrder,
                Contract = log.Contract,
                LogType = log.LogType.ToString(),
                MiningPool = log.MiningPool,
                StakingPool = log.StakingPool,
                Amount = log.Amount.InsertDecimal(TokenConstants.Opdex.Decimals)
            });
        }
    }
}
