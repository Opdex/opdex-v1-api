using MediatR;
using Opdex.Platform.Domain.Models.Tokens;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Tokens
{
    public class MakeTokenAttributeCommand : IRequest<bool>
    {
        public MakeTokenAttributeCommand(TokenAttribute tokenAttribute)
        {
            TokenAttribute = tokenAttribute ?? throw new ArgumentNullException(nameof(tokenAttribute), "Token attribute must be provided.");
        }

        public TokenAttribute TokenAttribute { get; }
    }
}
