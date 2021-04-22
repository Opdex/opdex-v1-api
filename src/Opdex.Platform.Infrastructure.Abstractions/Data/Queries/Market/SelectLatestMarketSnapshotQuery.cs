using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Market
{
    public class SelectLatestMarketSnapshotQuery : IRequest<MarketSnapshot>
    {
    }
}