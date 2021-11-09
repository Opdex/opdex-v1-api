using MediatR;
using Opdex.Platform.Application.Abstractions.Models.Tokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryQueries.Tokens.Snapshots
{
    /// <summary>
    /// Retrieves snapshot data from all markets for a given token.
    /// </summary>
    public class GetTokenSnapshotsWithFilterQuery : IRequest<TokenSnapshotsDto>
    {
        /// <summary>
        /// Creates a request to retrieve snapshot data from all markets for a given token.
        /// </summary>
        /// <param name="token">The address of the token.</param>
        /// <param name="cursor">The snapshot cursor filter.</param>
        public GetTokenSnapshotsWithFilterQuery(Address token, SnapshotCursor cursor)
        {
            if (token == Address.Empty) throw new ArgumentNullException(nameof(token), "Token address must not be empty.");
            if (cursor is null) throw new ArgumentNullException(nameof(cursor));

            Token = token;
            Cursor = cursor;
        }

        public Address Token { get; }

        public SnapshotCursor Cursor { get; }
    }
}
