using Opdex.Platform.Application.Abstractions.Models.TransactionEvents;
using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Vault;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.Vaults
{
    public class RedeemVaultCertificateEventDtoAssembler : IModelAssembler<RedeemVaultCertificateLog, RedeemVaultCertificateEventDto>
    {
        public Task<RedeemVaultCertificateEventDto> Assemble(RedeemVaultCertificateLog log)
        {
            return Task.FromResult(new RedeemVaultCertificateEventDto
            {
                Id = log.Id,
                TransactionId = log.TransactionId,
                SortOrder = log.SortOrder,
                Contract = log.Contract,
                EventType = TransactionEventType.RedeemVaultCertificateEvent,
                Amount = log.Amount.InsertDecimal(TokenConstants.Opdex.Decimals),
                Holder = log.Owner,
                VestedBlock = log.VestedBlock
            });
        }
    }
}
