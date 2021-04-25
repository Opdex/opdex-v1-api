using System;
using System.Collections.Generic;
using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens
{
    public class RetrieveActiveTokenSnapshotsByTokenIdQuery : IRequest<IEnumerable<TokenSnapshot>>
    {
        public RetrieveActiveTokenSnapshotsByTokenIdQuery(long tokenId)
        {
            if (tokenId < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(tokenId));
            }

            TokenId = tokenId;
        }
        
        public long TokenId { get; }
    }
}