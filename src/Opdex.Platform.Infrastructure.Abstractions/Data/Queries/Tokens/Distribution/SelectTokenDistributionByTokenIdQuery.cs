using System;
using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Tokens.Distribution
{
    public class SelectTokenDistributionByTokenIdQuery : IRequest<TokenDistribution>
    {
        public SelectTokenDistributionByTokenIdQuery(long tokenId)
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