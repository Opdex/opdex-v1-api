using System;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens
{
    public class SelectLatestTokenSnapshotByTokenIdQuery : IRequest<TokenSnapshot>
    {
        public SelectLatestTokenSnapshotByTokenIdQuery(long tokenId)
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