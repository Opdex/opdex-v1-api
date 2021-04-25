using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens
{
    public class SelectActiveTokenSnapshotsByTokenIdQuery : IRequest<IEnumerable<TokenSnapshot>>
    {
        public SelectActiveTokenSnapshotsByTokenIdQuery(long tokenId, DateTime now)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId));
            }

            if (now.Equals(default))
            {
                throw new ArgumentOutOfRangeException(nameof(now));
            }

            TokenId = tokenId;
            Now = now;
        }
        
        public long TokenId { get; }
        public DateTime Now { get; }
    }
}