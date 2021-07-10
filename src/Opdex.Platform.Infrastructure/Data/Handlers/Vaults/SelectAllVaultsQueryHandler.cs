using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vaults;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Vaults
{
    public class SelectAllVaultsQueryHandler : IRequestHandler<SelectAllVaultsQuery, IEnumerable<Vault>>
    {
        private static readonly string SqlQuery =
            @$"SELECT
                {nameof(VaultEntity.Id)},
                {nameof(VaultEntity.Address)},
                {nameof(VaultEntity.TokenId)},
                {nameof(VaultEntity.Owner)},
                {nameof(VaultEntity.Genesis)},
                {nameof(VaultEntity.UnassignedSupply)},
                {nameof(VaultEntity.CreatedBlock)},
                {nameof(VaultEntity.ModifiedBlock)}
            FROM vault;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectAllVaultsQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<Vault>> Handle(SelectAllVaultsQuery request, CancellationToken cancellationToken)
        {
            var query = DatabaseQuery.Create(SqlQuery, cancellationToken);

            var result = await _context.ExecuteQueryAsync<VaultEntity>(query);

            return _mapper.Map<IEnumerable<Vault>>(result);
        }
    }
}
