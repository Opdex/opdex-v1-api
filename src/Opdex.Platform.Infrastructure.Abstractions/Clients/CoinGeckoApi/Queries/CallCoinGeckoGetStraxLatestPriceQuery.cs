using MediatR;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CoinGeckoApi.Queries;

public class CallCoinGeckoGetStraxLatestPriceQuery : IRequest<decimal>
{
}
