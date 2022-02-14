using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.Auth;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Auth;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Auth;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Auth;

public class SelectAuthSuccessByConnectionIdQueryHandler : IRequestHandler<SelectAuthSuccessByConnectionIdQuery, AuthSuccess>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(AuthSuccessEntity.ConnectionId)},
                {nameof(AuthSuccessEntity.Signer)},
                {nameof(AuthSuccessEntity.Expiry)}
            FROM auth_success
            WHERE {nameof(AuthSuccessEntity.ConnectionId)} = @{nameof(SqlParams.ConnectionId)}
            LIMIT 1;".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectAuthSuccessByConnectionIdQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<AuthSuccess> Handle(SelectAuthSuccessByConnectionIdQuery request, CancellationToken cancellationToken)
    {
        var sqlParams = new SqlParams(request.ConnectionId);

        var query = DatabaseQuery.Create(SqlQuery, sqlParams, cancellationToken);

        var result = await _context.ExecuteFindAsync<AuthSuccessEntity>(query);

        return result is null ? null : _mapper.Map<AuthSuccess>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(string connectionId)
        {
            ConnectionId = connectionId;
        }

        public string ConnectionId { get; }
    }
}
