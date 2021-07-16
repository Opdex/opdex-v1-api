using Opdex.Platform.Application.Abstractions.Models.TransactionEvents.Deployers;
using Opdex.Platform.Domain.Models.TransactionLogs.MarketDeployers;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Assemblers.TransactionEvents.Deployers
{
    public class CreateMarketEventDtoAssembler : IModelAssembler<CreateMarketLog, CreateMarketEventDto>
    {
        public Task<CreateMarketEventDto> Assemble(CreateMarketLog log)
        {
            return Task.FromResult(new CreateMarketEventDto
            {
                Id = log.Id,
                TransactionId = log.TransactionId,
                SortOrder = log.SortOrder,
                Contract = log.Contract,
                LogType = log.LogType.ToString(),
                Market = log.Market,
                Owner = log.Owner,
                Router = log.Router,
                AuthPoolCreators = log.AuthPoolCreators,
                AuthProviders = log.AuthProviders,
                AuthTraders = log.AuthTraders,
                TransactionFee = log.TransactionFee,
                StakingToken = log.StakingToken,
                EnableMarketFee = log.EnableMarketFee
            });
        }
    }
}
