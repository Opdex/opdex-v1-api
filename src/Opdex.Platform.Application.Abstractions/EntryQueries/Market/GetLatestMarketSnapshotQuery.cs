using MediatR;
using Opdex.Platform.Application.Abstractions.Models;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Market
{
    public class GetLatestMarketSnapshotQuery : IRequest<MarketSnapshotDto>
    {
    }
}