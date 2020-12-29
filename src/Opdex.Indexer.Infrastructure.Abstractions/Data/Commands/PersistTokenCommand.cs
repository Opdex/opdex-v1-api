using System;
using MediatR;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands
{
    public class PersistTokenCommand : IRequest
    {
        public PersistTokenCommand(object token)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }
        
        public object Token { get; }
    }
}