using System;
using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks
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