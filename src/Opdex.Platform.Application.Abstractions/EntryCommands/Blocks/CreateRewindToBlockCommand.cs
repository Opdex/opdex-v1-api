using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Blocks
{
    public class CreateRewindToBlockCommand : IRequest<bool>
    {
        public CreateRewindToBlockCommand(ulong block)
        {
            if (block < 1)
            {
                throw new ArgumentNullException(nameof(block), "Block number must be greater than 0.");
            }

            Block = block;
        }

        public ulong Block { get; }
    }
}
