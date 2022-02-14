using MediatR;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Auth;
using System;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Auth;

public class SelectAuthSuccessByConnectionIdQuery : IRequest<AuthSuccess>
{
    public SelectAuthSuccessByConnectionIdQuery(string connectionId)
    {
        if (!connectionId.HasValue()) throw new ArgumentNullException(nameof(connectionId), "Connection Id must have a value.");

        ConnectionId = connectionId;
    }

    public string ConnectionId { get; }
}
