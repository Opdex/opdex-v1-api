using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Vault;
using Opdex.Platform.Common.Constants;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.TransactionLogs.Vaults;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.Vaults
{
    public class CreateVaultCertificateEventDtoAssembler : IModelAssembler<CreateVaultCertificateLog, CreateVaultCertificateEventDto>
    {
        public Task<CreateVaultCertificateEventDto> Assemble(CreateVaultCertificateLog log)
        {
            return Task.FromResult(new CreateVaultCertificateEventDto
            {
                Id = log.Id,
                TransactionId = log.TransactionId,
                SortOrder = log.SortOrder,
                Contract = log.Contract,
                LogType = log.LogType.ToString(),
                Amount = log.Amount.InsertDecimal(TokenConstants.Opdex.Decimals),
                Holder = log.Owner,
                VestedBlock = log.VestedBlock
            });
        }
    }
}
