using System;
using MediatR;
using Opdex.Core.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens
{
    public class SelectTokenByIdQuery : IRequest<Token>
    {
        public SelectTokenByIdQuery(long tokenId)
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