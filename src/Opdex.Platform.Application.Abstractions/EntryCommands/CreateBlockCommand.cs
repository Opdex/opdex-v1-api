using System;
using MediatR;
using Opdex.Platform.Domain.Models.Blocks;

namespace Opdex.Platform.Application.Abstractions.EntryCommands
{
    public class CreateBlockCommand : IRequest<bool>
    {
        public CreateBlockCommand(BlockReceipt blockReceipt)
        {
            BlockReceipt = blockReceipt ?? throw new ArgumentNullException(nameof(blockReceipt), $"{nameof(blockReceipt)} cannot be null.");
        }

        public BlockReceipt BlockReceipt { get; }
    }
}