using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens
{
    public class SelectActiveTokenSnapshotsByTokenIdQuery : IRequest<IEnumerable<TokenSnapshot>>
    {
        public SelectActiveTokenSnapshotsByTokenIdQuery(long tokenId, DateTime time)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId));
            }

            if (time.Equals(default))
            {
                throw new ArgumentOutOfRangeException(nameof(time));
            }

            TokenId = tokenId;
            Time = time;
        }
        
        public long TokenId { get; }
        public DateTime Time { get; }
    }
}