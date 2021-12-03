using System;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Application.Abstractions.Commands.Tokens;

public class MakeTokenSnapshotCommand : IRequest<bool>
{
    public MakeTokenSnapshotCommand(TokenSnapshot snapshot, ulong blockHeight)
    {
        if (blockHeight == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(blockHeight), "Block height must be greater than zero.");
        }

        BlockHeight = blockHeight;
        Snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot), "Token snapshot must be provided.");
    }

    public TokenSnapshot Snapshot { get; }
    public ulong BlockHeight { get; }
}