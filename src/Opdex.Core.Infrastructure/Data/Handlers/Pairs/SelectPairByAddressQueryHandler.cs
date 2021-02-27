using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Core.Common.Exceptions;
using Opdex.Core.Domain.Models;
using Opdex.Core.Infrastructure.Abstractions.Data;
using Opdex.Core.Infrastructure.Abstractions.Data.Models;
using Opdex.Core.Infrastructure.Abstractions.Data.Queries.Pairs;

namespace Opdex.Core.Infrastructure.Data.Handlers.Pairs
{
    public class SelectPairByAddressQueryHandler : IRequestHandler<SelectPairByAddressQuery, Pair>
    {
        private static readonly string SqlQuery = 
            @$"SELECT 
                {nameof(PairEntity.Id)},
                {nameof(PairEntity.Address)},
                {nameof(PairEntity.TokenId)},
                {nameof(PairEntity.ReserveCrs)},
                {nameof(PairEntity.ReserveSrc)}
            FROM pair
            WHERE {nameof(PairEntity.Address)} = @{nameof(SqlParams.Address)};";
                        
        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        
        public SelectPairByAddressQueryHandler(IDbContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        
        public async Task<Pair> Handle(SelectPairByAddressQuery request, CancellationToken cancellationToken)
        {
            var queryParams = new SqlParams(request.Address);
            var query = DatabaseQuery.Create(SqlQuery, queryParams, cancellationToken);
            
            var result = await _context.ExecuteFindAsync<PairEntity>(query);

            if (result == null)
            {
                throw new NotFoundException($"{nameof(PairEntity)} with address {request.Address} was not found.");
            }

            return _mapper.Map<Pair>(result);
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