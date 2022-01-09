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

public class SelectVaultGovernanceByTokenIdQueryHandler : IRequestHandler<SelectVaultGovernanceByTokenIdQuery, VaultGovernance>
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
            WHERE {nameof(VaultGovernanceEntity.TokenId)} = @{nameof(SqlParams.TokenId)}
            LIMIT 1;".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectVaultGovernanceByTokenIdQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<VaultGovernance> Handle(SelectVaultGovernanceByTokenIdQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.TokenId), cancellationToken);

        var result = await _context.ExecuteFindAsync<VaultGovernanceEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException($"{nameof(VaultGovernance)} not found.");
        }

        return result == null ? null : _mapper.Map<VaultGovernance>(result);
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

