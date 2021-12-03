using System;
using MediatR;
using Opdex.Platform.Domain.Models.Tokens;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Tokens;

public class PersistTokenSnapshotCommand : IRequest<bool>
{
    public PersistTokenSnapshotCommand(TokenSnapshot snapshot)
    {
        Snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
    }
        
    public TokenSnapshot Snapshot { get; }
}