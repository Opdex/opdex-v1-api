using System;
using MediatR;
using Opdex.Core.Domain.Models;

namespace Opdex.Indexer.Infrastructure.Abstractions.Data.Commands
{
    public class PersistBlockCommand : IRequest<bool>
    {
        public PersistBlockCommand(Block block)
        {
            Block = block ?? throw new ArgumentNullException(nameof(block));
        }
        
        public Block Block { get; }
    }
}