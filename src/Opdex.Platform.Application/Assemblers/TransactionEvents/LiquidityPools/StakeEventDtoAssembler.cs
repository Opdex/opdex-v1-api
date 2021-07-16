using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.LiquidityPools;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.LiquidityPools;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.LiquidityPools
{
    public class StakeEventDtoAssembler : IModelAssembler<StakeLog, StakeEventDto>
    {
        public Task<StakeEventDto> Assemble(StakeLog log)
        {
            return Task.FromResult(new StakeEventDto
            {
                Id = log.Id,
                TransactionId = log.TransactionId,
                SortOrder = log.SortOrder,
                Contract = log.Contract,
                LogType = log.LogType.ToString(),
                Amount = log.Amount.InsertDecimal(TokenConstants.Opdex.Decimals),
                Staker = log.Staker,
                EventType = log.EventType
            });
        }
    }
}
