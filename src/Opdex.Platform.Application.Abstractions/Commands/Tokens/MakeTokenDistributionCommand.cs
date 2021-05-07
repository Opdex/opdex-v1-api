using System;
using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Commands.Tokens
{
    public class MakeTokenDistributionCommand : IRequest<long>
    {
        public MakeTokenDistributionCommand(TokenDistribution tokenDistribution)
        {
            TokenDistribution = tokenDistribution ?? throw new ArgumentNullException(nameof(tokenDistribution));
        }
        
        public TokenDistribution TokenDistribution { get; }
    }
}