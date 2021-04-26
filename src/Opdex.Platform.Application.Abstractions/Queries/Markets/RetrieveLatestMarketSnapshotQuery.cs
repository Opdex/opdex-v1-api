using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets
{
    public class RetrieveLatestMarketSnapshotQuery : IRequest<MarketSnapshot>
    {
    }
}