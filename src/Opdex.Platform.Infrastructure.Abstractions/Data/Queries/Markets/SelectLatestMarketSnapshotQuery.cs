using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets
{
    public class SelectLatestMarketSnapshotQuery : IRequest<MarketSnapshot>
    {
    }
}