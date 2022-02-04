using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Tokens.Wrapped;

public class MakeTokenWrappedCommand : IRequest<ulong>
{
    public MakeTokenWrappedCommand(TokenWrapped tokenWrapped)
    {
        Wrapped = tokenWrapped ?? throw new ArgumentNullException(nameof(tokenWrapped));
    }

    public TokenWrapped Wrapped { get; }
}
