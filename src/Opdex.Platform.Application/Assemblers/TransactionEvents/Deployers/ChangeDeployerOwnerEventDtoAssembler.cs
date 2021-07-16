using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Deployers;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.Deployers
{
    public class ChangeDeployerOwnerEventDtoAssembler : IModelAssembler<ChangeDeployerOwnerLog, ChangeDeployerOwnerLogDto>
    {
        public Task<ChangeDeployerOwnerLogDto> Assemble(ChangeDeployerOwnerLog log)
        {
            return Task.FromResult(new ChangeDeployerOwnerLogDto
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
