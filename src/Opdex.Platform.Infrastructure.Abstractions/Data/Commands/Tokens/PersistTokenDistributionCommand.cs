using System;
using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens
{
    public class PersistTokenDistributionCommand : IRequest<long>
    {
        public PersistTokenDistributionCommand(TokenDistribution tokenDistribution)
        {
            TokenDistribution = tokenDistribution ?? throw new ArgumentNullException(nameof(tokenDistribution));
        }
        
        public TokenDistribution TokenDistribution { get; }
    }
}