using MediatR;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Application.Abstractions.EntryCommands.Blocks;

public class ProcessLatestBlocksCommand : IRequest<Unit>
{
    public ProcessLatestBlocksCommand(NetworkType networkType)
    {
        if (!networkType.IsValid())
        {
            throw new ArgumentOutOfRangeException(nameof(networkType), "Invalid network type.");
        }

        NetworkType = networkType;
    }

    public NetworkType NetworkType { get; }
}