using MediatR;
using Opdex.Platform.Application.Abstractions.Models;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Markets
{
    public class GetLatestMarketSnapshotQuery : IRequest<MarketSnapshotDto>
    {
    }
}