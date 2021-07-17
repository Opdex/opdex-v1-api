using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Deployers;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.Deployers
{
    public class ChangeDeployerOwnerEventDtoAssembler : IModelAssembler<ChangeDeployerOwnerLog, ChangeDeployerOwnerEventDto>
    {
        public Task<ChangeDeployerOwnerEventDto> Assemble(ChangeDeployerOwnerLog log)
        {
            return Task.FromResult(new ChangeDeployerOwnerEventDto
            {
                Id = log.Id,
                TransactionId = log.TransactionId,
                SortOrder = log.SortOrder,
                Contract = log.Contract,
                EventType = TransactionEventType.ChangeDeployerOwnerEvent,
                From = log.From,
                To = log.To
            });
        }
    }
}
