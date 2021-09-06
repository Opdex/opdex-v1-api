using System;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens
{
    public class PersistTokenDistributionCommand : IRequest<bool>
    {
        public PersistTokenDistributionCommand(TokenDistribution tokenDistribution)
        {
            TokenDistribution = tokenDistribution ?? throw new ArgumentNullException(nameof(tokenDistribution));
        }
        
        public TokenDistribution TokenDistribution { get; }
    }
}