using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.Markets
{
    public class ChangeMarketOwnerEventDtoAssembler : IModelAssembler<ChangeMarketOwnerLog, ChangeMarketOwnerEventDto>
    {
        public Task<ChangeMarketOwnerEventDto> Assemble(ChangeMarketOwnerLog log)
        {
            return Task.FromResult(new ChangeMarketOwnerEventDto
            {
                Id = log.Id,
                TransactionId = log.TransactionId,
                SortOrder = log.SortOrder,
                Contract = log.Contract,
                EventType = TransactionEventType.ChangeMarketOwnerEvent,
                From = log.From,
                To = log.To
            });
        }
    }
}
