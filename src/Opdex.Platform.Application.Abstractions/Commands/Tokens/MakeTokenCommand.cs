using System;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Abstractions.Commands.Tokens
{
    public class MakeTokenCommand : IRequest<long>
    {
        public MakeTokenCommand(Token token)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token), "Token must be provided.");
        }

        public Token Token { get; }
    }
}
