using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;

public class ProcessLatestBlocksCommand : IRequest<Unit>
{
    public ProcessLatestBlocksCommand(BlockReceipt block, NetworkType networkType)
    {
        if (block is null) throw new ArgumentNullException(nameof(block));
        if (!networkType.IsValid())
        {
            throw new ArgumentOutOfRangeException(nameof(networkType), "Invalid network type.");
        }

        Block = block;
        NetworkType = networkType;
    }

    public BlockReceipt Block { get; }
    public NetworkType NetworkType { get; }
}
