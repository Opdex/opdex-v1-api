using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens.Wrapped;

public class PersistTokenChainCommand : IRequest<ulong>
{
    public PersistTokenChainCommand(TokenChain tokenChain)
    {
        Chain = tokenChain ?? throw new ArgumentNullException(nameof(tokenChain));
    }

    public TokenChain Chain { get; }
}
