using MediatR;
using Opdex.Platform.Domain.Models.Markets;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets
{
    public class RetrieveLatestMarketSnapshotQuery : IRequest<MarketSnapshot>
    {
    }
}