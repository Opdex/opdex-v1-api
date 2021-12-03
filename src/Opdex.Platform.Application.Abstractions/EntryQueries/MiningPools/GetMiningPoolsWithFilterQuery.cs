using MediatR;
using Opdex.Platform.Application.Abstractions.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.MiningPools;

/// <summary>
/// Paginated request to retrieve a collection of mining pool and cursor details
/// </summary>
public class GetMiningPoolsWithFilterQuery : IRequest<MiningPoolsDto>
{
    /// <summary>
    /// Creates a request to retrieve a paged collection of mining pool details with a next and previous cursor
    /// </summary>
    public GetMiningPoolsWithFilterQuery(MiningPoolsCursor cursor)
    {
        Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
    }

    public MiningPoolsCursor Cursor { get; }
}