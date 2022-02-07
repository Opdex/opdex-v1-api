using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Blocks;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;

public class ProcessLatestBlocksCommand : IRequest<Unit>
{
    public ProcessLatestBlocksCommand(BlockReceipt currentBlock, NetworkType networkType, ulong notifyAfterHeight = 0)
    {
        if (currentBlock is null) throw new ArgumentNullException(nameof(currentBlock));
        if (!networkType.IsValid())
        {
            throw new ArgumentOutOfRangeException(nameof(networkType), "Invalid network type.");
        }

        CurrentBlock = currentBlock;
        NetworkType = networkType;
        NotifyAfterHeight = notifyAfterHeight;
    }

    public BlockReceipt CurrentBlock { get; }
    public NetworkType NetworkType { get; }
    public ulong NotifyAfterHeight { get; }
}
