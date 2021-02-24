using System;
using MediatR;
using Opdex.Core.Domain.Models;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands
{
    public class PersistPairCommand : IRequest<long>
    {
        public PersistPairCommand(Pair pair)
        {
            Pair = pair ?? throw new ArgumentNullException(nameof(pair));
        }
        
        public Pair Pair { get;  }
    }
}