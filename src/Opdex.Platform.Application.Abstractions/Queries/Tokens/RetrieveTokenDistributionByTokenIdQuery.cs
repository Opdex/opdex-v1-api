using System;
using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Queries.Tokens
{
    public class RetrieveTokenDistributionByTokenIdQuery : IRequest<TokenDistribution>
    {
        public RetrieveTokenDistributionByTokenIdQuery(long tokenId)
        {
            if (tokenId < 1)
            {
                throw new ArgumentNullException(nameof(tokenId));
            }

            TokenId = tokenId;
        }
        
        public long TokenId { get; }
    }
}