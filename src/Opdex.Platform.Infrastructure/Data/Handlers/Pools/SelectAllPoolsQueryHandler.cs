using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pools;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Pools
{
    public class SelectAllPoolsQueryHandler : IRequestHandler<SelectAllPoolsQuery, IEnumerable<Pool>>
    {
        private static readonly string SqlCommand =
            $@"SELECT
                {nameof(PoolEntity.Id)},
                {nameof(PoolEntity.Address)},
                {nameof(PoolEntity.TokenId)},
                {nameof(PoolEntity.ReserveCrs)},
                {nameof(PoolEntity.ReserveSrc)},
                {nameof(PoolEntity.CreatedDate)}
            FROM pool;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public SelectAllPoolsQueryHandler(IDbContext context, IMapper mapper, 
            ILogger<SelectAllPoolsQueryHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<IEnumerable<Pool>> Handle(SelectAllPoolsQuery request, CancellationToken cancellationToken)
        {
            var command = DatabaseQuery.Create(SqlCommand, null, cancellationToken);
            
            var tokenEntities =  await _context.ExecuteQueryAsync<PoolEntity>(command);

            return _mapper.Map<IEnumerable<Pool>>(tokenEntities);
        }
    }
}