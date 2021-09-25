using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Tokens.Distribution
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
