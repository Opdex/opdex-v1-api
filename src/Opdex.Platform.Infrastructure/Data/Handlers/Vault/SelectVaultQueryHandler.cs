using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.ODX;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Vault;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Vault
{
    public class SelectVaultQueryHandler : IRequestHandler<SelectVaultQuery, Domain.Models.ODX.Vault>
    {
        private static readonly string SqlQuery =
            @$"SELECT 
                {nameof(VaultEntity.Id)},
                {nameof(VaultEntity.Address)},
                {nameof(VaultEntity.TokenId)},
                {nameof(VaultEntity.Owner)},
                {nameof(VaultEntity.Genesis)},
                {nameof(VaultEntity.CreatedBlock)},
                {nameof(VaultEntity.ModifiedBlock)}
            FROM odx_vault
            LIMIT 1;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectVaultQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<Domain.Models.ODX.Vault> Handle(SelectVaultQuery request, CancellationToken cancellationToken)
        {
            var query = DatabaseQuery.Create(SqlQuery, cancellationToken);

            var result = await _context.ExecuteFindAsync<VaultEntity>(query);

            if (request.FindOrThrow && result == null)
            {
                throw new NotFoundException($"{nameof(Vault)} not found.");
            }

            return result == null ? null : _mapper.Map<Domain.Models.ODX.Vault>(result);
        }
    }
}