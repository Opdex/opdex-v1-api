using MediatR;
using Opdex.Platform.Common.Models;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.SignalR.Commands
{
    public class NotifyUserOfMinedTransactionCommand : IRequest
    {
        public NotifyUserOfMinedTransactionCommand(Address user, Sha256 txHash)
        {
            User = user != Address.Empty ? user : throw new ArgumentNullException(nameof(user), "User address must be set.");
            TxHash = txHash;
        }

        public Address User { get; }
        public Sha256 TxHash { get; }
    }
}
