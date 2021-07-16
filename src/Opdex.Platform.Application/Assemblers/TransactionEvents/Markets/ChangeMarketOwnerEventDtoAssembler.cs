using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.Markets
{
    public class ChangeMarketOwnerEventDtoAssembler : IModelAssembler<ChangeMarketOwnerLog, ChangeMarketOwnerLogDto>
    {
        public Task<ChangeMarketOwnerLogDto> Assemble(ChangeMarketOwnerLog log)
        {
            return Task.FromResult(new ChangeMarketOwnerLogDto
            {
                Id = log.Id,
                TransactionId = log.TransactionId,
                SortOrder = log.SortOrder,
                Contract = log.Contract,
                LogType = log.LogType.ToString(),
                From = log.From,
                To = log.To
            });
        }
    }
}
