using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens.Wrapped;

public class PersistTokenWrappedCommand : IRequest<ulong>
{
    public PersistTokenWrappedCommand(TokenWrapped tokenWrapped)
    {
        Wrapped = tokenWrapped ?? throw new ArgumentNullException(nameof(tokenWrapped));
    }

    public TokenWrapped Wrapped { get; }
}
