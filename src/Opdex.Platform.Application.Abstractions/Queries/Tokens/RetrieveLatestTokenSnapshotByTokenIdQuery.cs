using System;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens
{
    public class RetrieveLatestTokenSnapshotByTokenIdQuery : IRequest<TokenSnapshot>
    {
        public RetrieveLatestTokenSnapshotByTokenIdQuery(long tokenId)
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
    
   