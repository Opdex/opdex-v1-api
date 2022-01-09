using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Vaults;

public class SelectVaultGovernanceByAddressQueryHandler : IRequestHandler<SelectVaultGovernanceByAddressQuery, VaultGovernance>
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
            WHERE {nameof(VaultGovernanceEntity.Address)} = @{nameof(SqlParams.VaultAddress)}
            LIMIT 1;".RemoveExcessWhitespace();

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectVaultGovernanceByAddressQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<VaultGovernance> Handle(SelectVaultGovernanceByAddressQuery request, CancellationToken cancellationToken)
    {
        var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.Vault), cancellationToken);

        var result = await _context.ExecuteFindAsync<VaultGovernanceEntity>(query);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException($"{nameof(VaultGovernance)} not found.");
        }

        return result == null ? null : _mapper.Map<VaultGovernance>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(Address vaultAddress)
        {
            VaultAddress = vaultAddress;
        }

        public Address VaultAddress { get; }
    }
}
