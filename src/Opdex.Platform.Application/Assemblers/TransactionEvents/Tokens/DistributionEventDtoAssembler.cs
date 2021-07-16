using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Tokens;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.Tokens;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.Tokens
{
    public class DistributionEventDtoAssembler : IModelAssembler<DistributionLog, DistributionEventDto>
    {
        public Task<DistributionEventDto> Assemble(DistributionLog log)
        {
            return Task.FromResult(new DistributionEventDto
            {
                Id = log.Id,
                TransactionId = log.TransactionId,
                SortOrder = log.SortOrder,
                Contract = log.Contract,
                LogType = log.LogType.ToString(),
                MiningAmount = log.MiningAmount.InsertDecimal(TokenConstants.Opdex.Decimals),
                VaultAmount = log.VaultAmount.InsertDecimal(TokenConstants.Opdex.Decimals),
                PeriodIndex = log.PeriodIndex
            });
        }
    }
}
