using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Market
{
    public class RetrieveLatestMarketSnapshotQuery : IRequest<MarketSnapshot>
    {
    }
}