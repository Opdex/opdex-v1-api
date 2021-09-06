using MediatR;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Blocks
{
    public class MakeRewindToBlockCommand : IRequest<bool>
    {
        public MakeRewindToBlockCommand(ulong block)
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
