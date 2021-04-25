using System;
using MediatR;
using Opdex.Platform.Domain.Models;

namespace Opdex.Platform.Application.Abstractions.Commands.Tokens
{
    public class MakeTokenSnapshotCommand : IRequest<bool>
    {
        public MakeTokenSnapshotCommand(TokenSnapshot snapshot)
        {
            Snapshot = snapshot ?? throw new ArgumentNullException(nameof(snapshot));
        }
        
        public TokenSnapshot Snapshot { get; }
    }
}