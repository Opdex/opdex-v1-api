using MediatR;
using Opdex.Platform.Domain.Models.Auth;

namespace Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Auth;

public class SelectAuthSuccessByConnectionIdQuery : IRequest<AuthSuccess>
{
    public SelectAuthSuccessByConnectionIdQuery(string connectionId)
    {
        ConnectionId = connectionId;
    }

    public string ConnectionId { get; }
}
