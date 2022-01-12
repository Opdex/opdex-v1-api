using MediatR;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CoinGeckoApi.Queries;

public class CallCoinGeckoGetStraxHistoricalPriceQuery : IRequest<decimal>
{
    public CallCoinGeckoGetStraxHistoricalPriceQuery(DateTime dateTime)
    {
        if (dateTime == default || dateTime > DateTime.UtcNow)
        {
            throw new ArgumentOutOfRangeException(nameof(dateTime), "Datetime must be set and in the past");
        }

        DateTime = dateTime;
    }

    public DateTime DateTime { get; }
}
