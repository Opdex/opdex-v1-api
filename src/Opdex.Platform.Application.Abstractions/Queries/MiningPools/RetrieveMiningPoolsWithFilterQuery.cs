using MediatR;
using Opdex.Platform.Domain.Models.MiningPools;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.MiningPools;
using System;
using System.Collections.Generic;

namespace Opdex.Platform.Application.Abstractions.Queries.MiningPools
{
    /// <summary>
    /// Paginated request to retrieve a collection of mining pools
    /// </summary>
    public class RetrieveMiningPoolsWithFilterQuery : IRequest<IEnumerable<MiningPool>>
    {
        /// <summary>
        /// Creates a request to retrieve a paged collection of mining pools
        /// </summary>
        public RetrieveMiningPoolsWithFilterQuery(MiningPoolsCursor cursor)
        {
            Cursor = cursor ?? throw new ArgumentNullException(nameof(cursor), "Cursor must be set.");
        }

        public MiningPoolsCursor Cursor { get; }
    }
}
