using MediatR;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.CoinMarketCapApi.Queries;

/// <summary>
/// Retrieve a historical STRAX price from Coin Market Cap.
/// </summary>
public class CallCmcGetStraxHistoricalQuoteQuery : IRequest<decimal>
{
    /// <summary>
    /// Constructor to build a call CMC get STRAX historical quote query.
    /// </summary>
    /// <param name="dateTime">The datetime to get the price at.</param>
    public CallCmcGetStraxHistoricalQuoteQuery(DateTime dateTime)
    {
        if (dateTime == default || dateTime > DateTime.UtcNow)
        {
            throw new ArgumentOutOfRangeException(nameof(dateTime), "Datetime must be set and in the past");
        }

        DateTime = dateTime;
    }

    public DateTime DateTime { get; }
}
