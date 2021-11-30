using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models;
using System;

namespace Opdex.Platform.Application.Abstractions.Commands.Indexer;

public class MakeIndexerLockCommand : IRequest
{
    public MakeIndexerLockCommand(IndexLockReason reason)
    {
        if (!reason.IsValid()) throw new ArgumentOutOfRangeException(nameof(reason), "Reason must be valid.");
        Reason = reason;
    }

    public IndexLockReason Reason { get; }
}
