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

public class SelectVaultByIdQueryHandler : IRequestHandler<SelectVaultByIdQuery, Vault>
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
            WHERE {nameof(VaultEntity.Id)} = @{nameof(SqlParams.VaultId)}
            LIMIT 1;".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectVaultByIdQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Vault> Handle(SelectVaultByIdQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.VaultId), cancellationToken);

        var result = await _context.ExecuteFindAsync<VaultEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException($"{nameof(Vault)} not found.");
        }

        return result == null ? null : _mapper.Map<Vault>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(ulong vaultId)
        {
            VaultId = vaultId;
        }

        public ulong VaultId { get; }
    }
}
