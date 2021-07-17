using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Vault;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.Vaults
{
    public class RevokeVaultCertificateEventDtoAssembler : IModelAssembler<RevokeVaultCertificateLog, RevokeVaultCertificateEventDto>
    {
        public Task<RevokeVaultCertificateEventDto> Assemble(RevokeVaultCertificateLog log)
        {
            return Task.FromResult(new RevokeVaultCertificateEventDto
            {
                Id = log.Id,
                TransactionId = log.TransactionId,
                SortOrder = log.SortOrder,
                Contract = log.Contract,
                EventType = TransactionEventType.RevokeVaultCertificateEvent,
                NewAmount = log.NewAmount.InsertDecimal(TokenConstants.Opdex.Decimals),
                OldAmount = log.OldAmount.InsertDecimal(TokenConstants.Opdex.Decimals),
                Holder = log.Owner,
                VestedBlock = log.VestedBlock
            });
        }
    }
}
