using MediatR;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Queries
{
    public class CallCmcGetStraxQuotePriceQuery : IRequest<decimal>
    {
        // Todo: Move to config
        public int TokenId => 1343;
    }
}