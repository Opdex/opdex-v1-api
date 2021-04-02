using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Common.Exceptions;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Pools;

namespace Opdex.Core.Infrastructure.Data.Handlers.Pools
{
    public class SelectPoolByAddressQueryHandler : IRequestHandler<SelectPoolByAddressQuery, Pool>
    {
        private static readonly string SqlQuery = 
            @$"SELECT 
                {nameof(PoolEntity.Id)},
                {nameof(PoolEntity.Address)},
                {nameof(PoolEntity.TokenId)},
                {nameof(PoolEntity.ReserveCrs)},
                {nameof(PoolEntity.ReserveSrc)}
            FROM pool
            WHERE {nameof(PoolEntity.Address)} = @{nameof(SqlParams.Address)};";
                        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        
        public SelectPoolByAddressQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<Pool> Handle(SelectPoolByAddressQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.Address);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);
            
            var result = await _context.ExecuteFindAsync<PoolEntity>(query);

            if (result == null)
            {
                throw new NotFoundException($"{nameof(PoolEntity)} with address {request.Address} was not found.");
            }

            return _mapper.Map<Pool>(result);
        }

        private sealed class SqlParams
        {
            internal SqlParams(string address)
            {
                Address = address;
            }
            
            public string Address { get; }
        }
    }
}