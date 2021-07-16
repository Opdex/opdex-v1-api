using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.MiningPools;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.MiningPools;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.MiningPools
{
    public class EnableMiningEventDtoAssembler : IModelAssembler<EnableMiningLog, EnableMiningLogDto>
    {
        public Task<EnableMiningLogDto> Assemble(EnableMiningLog source)
        {
            return Task.FromResult(new EnableMiningLogDto
            {
                Id = source.Id,
                TransactionId = source.TransactionId,
                SortOrder = source.SortOrder,
                Amount = source.Amount.InsertDecimal(TokenConstants.Opdex.Decimals),
                Contract = source.Contract,
                LogType = source.LogType.ToString(),
                MiningPeriodEndBlock = source.MiningPeriodEndBlock,
                RewardRate = source.RewardRate,
            });
        }
    }
}
