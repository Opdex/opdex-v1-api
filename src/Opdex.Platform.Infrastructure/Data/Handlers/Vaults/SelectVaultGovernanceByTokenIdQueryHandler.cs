using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Vaults;

public class SelectVaultByTokenIdQueryHandler : IRequestHandler<SelectVaultByTokenIdQuery, Vault>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(VaultEntity.Id)},
                {nameof(VaultEntity.Address)},
                {nameof(VaultEntity.TokenId)},
                {nameof(VaultEntity.UnassignedSupply)},
                {nameof(VaultEntity.ProposedSupply)},
                {nameof(VaultEntity.VestingDuration)},
                {nameof(VaultEntity.TotalPledgeMinimum)},
                {nameof(VaultEntity.TotalVoteMinimum)},
                {nameof(VaultEntity.CreatedBlock)},
                {nameof(VaultEntity.ModifiedBlock)}
            FROM vault
            WHERE {nameof(VaultEntity.TokenId)} = @{nameof(SqlParams.TokenId)}
            LIMIT 1;".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectVaultByTokenIdQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Vault> Handle(SelectVaultByTokenIdQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.TokenId), cancellationToken);

        var result = await _context.ExecuteFindAsync<VaultEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException($"{nameof(Vault)} not found.");
        }

        return result == null ? null : _mapper.Map<Vault>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong tokenId)
        {
            TokenId = tokenId;
        }

        public ulong TokenId { get; }
    }
}

