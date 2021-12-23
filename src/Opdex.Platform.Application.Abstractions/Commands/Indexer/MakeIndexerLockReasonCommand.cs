using MediatR;
using Opdex.Platform.Domain.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Indexer;

public class MakeIndexerLockReasonCommand : IRequest
{
    public MakeIndexerLockReasonCommand(IndexLockReason reason)
    {
        if (reason is IndexLockReason.Deploying or IndexLockReason.Indexing or IndexLockReason.Searching)
        {
            throw new ArgumentOutOfRangeException(nameof(reason), "Reason can only be updated to rewinding or resyncing");
        }
        Reason = reason;
    }

    public IndexLockReason Reason { get; }
}
