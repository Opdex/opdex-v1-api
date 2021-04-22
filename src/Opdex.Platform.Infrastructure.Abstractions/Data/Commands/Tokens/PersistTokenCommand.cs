using System;
using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands
{
    public class PersistTokenCommand : IRequest<long>
    {
        public PersistTokenCommand(Token token)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }
        
        public Token Token { get; }
    }
}