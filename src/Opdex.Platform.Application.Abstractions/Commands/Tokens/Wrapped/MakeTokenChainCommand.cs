using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Tokens.Wrapped;

public class MakeTokenChainCommand : IRequest<ulong>
{
    public MakeTokenChainCommand(TokenChain tokenChain)
    {
        Chain = tokenChain ?? throw new ArgumentNullException(nameof(tokenChain));
    }

    public TokenChain Chain { get; }
}
