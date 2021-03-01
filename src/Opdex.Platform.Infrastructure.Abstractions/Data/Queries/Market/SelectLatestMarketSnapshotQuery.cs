using MediatR;
using Opdex.Core.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Market
{
    public class SelectLatestMarketSnapshotQuery : IRequest<MarketSnapshot>
    {
    }
}