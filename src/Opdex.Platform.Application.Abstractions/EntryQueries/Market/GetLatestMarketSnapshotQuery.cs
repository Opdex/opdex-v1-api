using MediatR;
using Opdex.Core.Application.Abstractions.Models;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Market
{
    public class GetLatestMarketSnapshotQuery : IRequest<MarketSnapshotDto>
    {
    }
}