using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Abstractions.Feeds;

public interface IFiatPriceFeed
{
    Task<decimal> GetCrsUsdPrice(DateTime blockTime, CancellationToken cancellationToken = default);
}
