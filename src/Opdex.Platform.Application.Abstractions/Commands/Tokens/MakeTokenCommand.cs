using System;
using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Abstractions.Commands.Tokens
{
    public class MakeTokenCommand : IRequest<long>
    {
        public MakeTokenCommand(string address, Token token = null)
        {
            if (!address.HasValue() && token == null)
            {
                throw new ArgumentException("Either address or token must not be null");
            }

            Address = address;
            Token = token;
        }

        public string Address { get; }
        public Token Token { get; }
    }
}