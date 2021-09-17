using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Extensions;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Deployers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Deployers
{
    public class SelectDeployersByModifiedBlockQueryHandler : IRequestHandler<SelectDeployersByModifiedBlockQuery, IEnumerable<Deployer>>
    {
        private static readonly string SqlQuery =
            $@"SELECT
                {nameof(DeployerEntity.Id)},
                {nameof(DeployerEntity.Address)},
                {nameof(DeployerEntity.Owner)},
                {nameof(DeployerEntity.CreatedBlock)},
                {nameof(DeployerEntity.ModifiedBlock)}
            FROM market_deployer
            WHERE {nameof(DeployerEntity.ModifiedBlock)} = @{nameof(SqlParams.ModifiedBlock)};".RemoveExcessWhitespace();

        private readonly IDbContext _context;
        private readonly IMapper _mapper;

        public SelectDeployersByModifiedBlockQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<IEnumerable<Deployer>> Handle(SelectDeployersByModifiedBlockQuery request, CancellationToken cancellationToken)
        {
            var query = DatabaseQuery.Create(SqlQuery, new SqlParams(request.BlockHeight), cancellationToken);

            var result = await _context.ExecuteQueryAsync<DeployerEntity>(query);

            return _mapper.Map<IEnumerable<Deployer>>(result);
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
