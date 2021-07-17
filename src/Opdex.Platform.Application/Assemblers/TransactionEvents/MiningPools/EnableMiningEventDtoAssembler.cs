using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.MiningPools
{
    public class EnableMiningEventDtoAssembler : IModelAssembler<EnableMiningLog, EnableMiningEventDto>
    {
        public Task<EnableMiningEventDto> Assemble(EnableMiningLog log)
        {
            return Task.FromResult(new EnableMiningEventDto
            {
                Id = log.Id,
                TransactionId = log.TransactionId,
                SortOrder = log.SortOrder,
                Amount = log.Amount.InsertDecimal(TokenConstants.Opdex.Decimals),
                Contract = log.Contract,
                EventType = TransactionEventType.EnableMiningEvent,
                MiningPeriodEndBlock = log.MiningPeriodEndBlock,
                RewardRate = log.RewardRate.InsertDecimal(TokenConstants.Opdex.Decimals)
            });
        }
    }
}
