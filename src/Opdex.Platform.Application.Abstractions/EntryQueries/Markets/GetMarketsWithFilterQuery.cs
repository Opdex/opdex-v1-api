using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Markets;

public class GetMarketsWithFilterQuery : IRequest<MarketsDto>
{
    public GetMarketsWithFilterQuery(MarketsCursor cursor)
    {
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor));
    }

    public MarketsCursor Cursor { get; }
}
