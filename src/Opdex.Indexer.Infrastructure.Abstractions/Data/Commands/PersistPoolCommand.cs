using System;
using MediatR;
using Opdex.Core.Domain.Models;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands
{
    public class PersistPoolCommand : IRequest<long>
    {
        public PersistPoolCommand(Pool pool)
        {
            Pool = pool ?? throw new ArgumentNullException(nameof(pool));
        }
        
        public Pool Pool { get;  }
    }
}