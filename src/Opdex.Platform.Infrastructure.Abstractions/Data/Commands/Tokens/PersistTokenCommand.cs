using System;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens
{
    public class PersistTokenCommand : IRequest<ulong>
    {
        public PersistTokenCommand(Token token)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }

        public Token Token { get; }
    }
}
