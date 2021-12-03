using MediatR;
using Opdex.Platform.Common.Extensions;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Clients.SignalR.Commands;

public class NotifyUserOfSuccessfulAuthenticationCommand : IRequest
{
    public NotifyUserOfSuccessfulAuthenticationCommand(string connectionId, string bearerToken)
    {
        ConnectionId = connectionId.HasValue() ? connectionId : throw new ArgumentNullException(nameof(connectionId), "Connection id must be set.");
        BearerToken = bearerToken.HasValue() ? bearerToken : throw new ArgumentNullException(nameof(bearerToken), "Bearer token must be set.");
    }

    public string ConnectionId { get; }
    public string BearerToken { get; }
}