using System;
using MediatR;
using Opdex.Platform.Domain.Models.ODX;

namespace Opdex.Platform.Application.Abstractions.Commands.Tokens
{
    public class MakeTokenDistributionCommand : IRequest<bool>
    {
        public MakeTokenDistributionCommand(TokenDistribution tokenDistribution)
        {
            TokenDistribution = tokenDistribution ?? throw new ArgumentNullException(nameof(tokenDistribution));
        }
        
        public TokenDistribution TokenDistribution { get; }
    }
}