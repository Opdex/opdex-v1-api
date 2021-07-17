using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Vault;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.Vaults
{
    public class ChangeVaultOwnerEventDtoAssembler : IModelAssembler<ChangeVaultOwnerLog, ChangeVaultOwnerEventDto>
    {
        public Task<ChangeVaultOwnerEventDto> Assemble(ChangeVaultOwnerLog log)
        {
            return Task.FromResult(new ChangeVaultOwnerEventDto
            {
                Id = log.Id,
                TransactionId = log.TransactionId,
                SortOrder = log.SortOrder,
                Contract = log.Contract,
                EventType = TransactionEventType.ChangeVaultOwnerEvent,
                From = log.From,
                To = log.To
            });
        }
    }
}
