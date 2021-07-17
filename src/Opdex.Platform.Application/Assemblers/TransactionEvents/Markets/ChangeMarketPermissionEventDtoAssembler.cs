using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Markets;
using Opdex.Platform.Domain.Models.TransactionLogs.Markets;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.Markets
{
    public class ChangeMarketPermissionEventDtoAssembler : IModelAssembler<ChangeMarketPermissionLog, ChangeMarketPermissionEventDto>
    {
        public Task<ChangeMarketPermissionEventDto> Assemble(ChangeMarketPermissionLog log)
        {
            return Task.FromResult(new ChangeMarketPermissionEventDto
            {
                Id = log.Id,
                TransactionId = log.TransactionId,
                SortOrder = log.SortOrder,
                Contract = log.Contract,
                EventType = TransactionEventType.ChangeMarketPermissionEvent,
                Address = log.Address,
                Permission = log.Permission.ToString(),
                IsAuthorized = log.IsAuthorized
            });
        }
    }
}
