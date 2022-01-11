using MediatR;
using Opdex.Platform.Domain.Models.Markets;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;

public class SelectMarketsWithFilterQuery : IRequest<IEnumerable<Market>>
{
    public SelectMarketsWithFilterQuery(MarketsCursor cursor)
    {
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor));
    }

    public MarketsCursor Cursor { get; }
}
