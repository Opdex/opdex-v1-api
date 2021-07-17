using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.MiningPools
{
    public class MineEventDtoAssembler : IModelAssembler<MineLog, MineEventDto>
    {
        public Task<MineEventDto> Assemble(MineLog log)
        {
            return Task.FromResult(new MineEventDto
            {
                Id = log.Id,
                TransactionId = log.TransactionId,
                SortOrder = log.SortOrder,
                Contract = log.Contract,
                Amount = log.Amount.InsertDecimal(TokenConstants.LiquidityPoolToken.Decimals),
                Miner = log.Miner,
                EventType = TransactionEventType.MineEvent,
                SubEventType = log.EventType == 1 ? "StartMining" : "StopMining"
            });
        }
    }
}
