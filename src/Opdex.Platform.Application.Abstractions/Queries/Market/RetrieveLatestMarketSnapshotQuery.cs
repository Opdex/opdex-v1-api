using MediatR;
using Opdex.Core.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Market
{
    public class RetrieveLatestMarketSnapshotQuery : IRequest<MarketSnapshot>
    {
    }
}