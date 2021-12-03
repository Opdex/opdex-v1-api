using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Indexer;

public class PersistIndexerLockCommand : IRequest<bool>
{
    public PersistIndexerLockCommand(IndexLockReason reason)
    {
        if (!reason.IsValid()) throw new ArgumentOutOfRangeException(nameof(reason), "Reason must be valid.");
        Reason = reason;
    }

    public IndexLockReason Reason { get; }
}
