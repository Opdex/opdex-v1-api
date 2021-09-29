using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Vaults
{
    public class SelectVaultsByModifiedBlockQueryHandler : IRequestHandler<SelectVaultsByModifiedBlockQuery, IEnumerable<Vault>>
    {
        private static readonly string SqlQuery =
            @$"SELECT
                {nameof(VaultEntity.Id)},
                {nameof(VaultEntity.Address)},
                {nameof(VaultEntity.TokenId)},
                {nameof(VaultEntity.PendingOwner)},
                {nameof(VaultEntity.Owner)},
                {nameof(VaultEntity.Genesis)},
                {nameof(VaultEntity.UnassignedSupply)},
                {nameof(VaultEntity.CreatedBlock)},
                {nameof(VaultEntity.ModifiedBlock)}
            FROM vault
            WHERE {nameof(VaultEntity.ModifiedBlock)} = @{nameof(SqlParams.ModifiedBlock)};".RemoveExcessWhitespace();

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectVaultsByModifiedBlockQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<Vault>> Handle(SelectVaultsByModifiedBlockQuery request, CancellationToken cancellationToken)
        {
            var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.BlockHeight), cancellationToken);

            var result = await _context.ExecuteQueryAsync<VaultEntity>(query);

            return _mapper.Map<IEnumerable<Vault>>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(ulong modifiedBlock)
            {
                ModifiedBlock = modifiedBlock;
            }

            public ulong ModifiedBlock { get; }
        }
    }
}
