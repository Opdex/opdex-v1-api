using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.VaultGovernances;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.VaultGovernances;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.VaultGovernances;

public class SelectVaultGovernanceByIdQueryHandler : IRequestHandler<SelectVaultGovernanceByIdQuery, VaultGovernance>
{
    private static readonly string SqlQuery =
        @$"SELECT
                {nameof(VaultGovernanceEntity.Id)},
                {nameof(VaultGovernanceEntity.Address)},
                {nameof(VaultGovernanceEntity.TokenId)},
                {nameof(VaultGovernanceEntity.UnassignedSupply)},
                {nameof(VaultGovernanceEntity.ProposedSupply)},
                {nameof(VaultGovernanceEntity.VestingDuration)},
                {nameof(VaultGovernanceEntity.TotalPledgeMinimum)},
                {nameof(VaultGovernanceEntity.TotalVoteMinimum)},
                {nameof(VaultGovernanceEntity.CreatedBlock)},
                {nameof(VaultGovernanceEntity.ModifiedBlock)}
            FROM vault
            WHERE {nameof(VaultGovernanceEntity.Id)} = @{nameof(SqlParams.VaultId)}
            LIMIT 1;".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectVaultGovernanceByIdQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<VaultGovernance> Handle(SelectVaultGovernanceByIdQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.VaultId), cancellationToken);

        var result = await _context.ExecuteFindAsync<VaultGovernanceEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException($"{nameof(VaultGovernance)} not found.");
        }

        return result == null ? null : _mapper.Map<VaultGovernance>(result);
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
