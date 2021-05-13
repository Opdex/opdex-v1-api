using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens
{
    public class RetrieveActiveTokenSnapshotsByTokenIdQuery : IRequest<IEnumerable<TokenSnapshot>>
    {
        public RetrieveActiveTokenSnapshotsByTokenIdQuery(long tokenId, DateTime time)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId));
            }

            if (time.Equals(default))
            {
                throw new ArgumentException(nameof(time));
            }

            TokenId = tokenId;
            Time = time;
        }
        
        public long TokenId { get; }
        public DateTime Time { get; }
    }
}