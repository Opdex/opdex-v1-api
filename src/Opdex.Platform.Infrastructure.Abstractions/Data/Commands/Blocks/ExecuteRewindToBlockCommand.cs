using MediatR;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Blocks
{
    public class ExecuteRewindToBlockCommand : IRequest<bool>
    {
        public ExecuteRewindToBlockCommand(ulong block)
        {
            if (block < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(block), "Block number must be greater than 0.");
            }

            Block = block;
        }

        public ulong Block { get; }
    }
}
