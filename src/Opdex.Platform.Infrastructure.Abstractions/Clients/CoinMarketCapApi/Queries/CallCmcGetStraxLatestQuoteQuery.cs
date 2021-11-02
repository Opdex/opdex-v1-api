using MediatR;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Queries
{
    /// <summary>
    /// Retrieve the current STRAX price from Coin Market Cap.
    /// </summary>
    public class CallCmcGetStraxLatestQuoteQuery : IRequest<decimal>
    {
    }
}
