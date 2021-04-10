using System;
using MediatR;
using Opdex.Core.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens
{
    public class RetrieveTokenByIdQuery : IRequest<Token>
    {
        public RetrieveTokenByIdQuery(long tokenId)
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