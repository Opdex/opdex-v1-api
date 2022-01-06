using MediatR;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.Markets;

public class RetrieveMarketsWithFilterQuery : IRequest<IEnumerable<Market>>
{
    public RetrieveMarketsWithFilterQuery(MarketsCursor cursor)
    {
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor));
    }

    public MarketsCursor Cursor { get; }
}
