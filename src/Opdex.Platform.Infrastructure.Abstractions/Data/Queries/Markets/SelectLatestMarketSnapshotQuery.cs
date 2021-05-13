using MediatR;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets
{
    public class SelectLatestMarketSnapshotQuery : IRequest<MarketSnapshot>
    {
    }
}