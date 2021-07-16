using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.MiningPools
{
    public class MineEventDtoAssembler : IModelAssembler<MineLog, MineLogDto>
    {
        public Task<MineLogDto> Assemble(MineLog log)
        {
            return Task.FromResult(new MineLogDto
            {
                Id = log.Id,
                TransactionId = log.TransactionId,
                SortOrder = log.SortOrder,
                Contract = log.Contract,
                LogType = log.LogType.ToString(),
                Amount = log.Amount.InsertDecimal(TokenConstants.LiquidityPoolToken.Decimals),
                Miner = log.Miner,
                EventType = log.EventType.ToString()
            });
        }
    }
}
