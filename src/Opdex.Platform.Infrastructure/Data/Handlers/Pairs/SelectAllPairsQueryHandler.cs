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
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Pairs;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Pairs
{
    public class SelectAllPairsQueryHandler : IRequestHandler<SelectAllPairsQuery, IEnumerable<Pair>>
    {
        private static readonly string SqlCommand =
            $@"SELECT
                {nameof(PairEntity.Id)},
                {nameof(PairEntity.Address)},
                {nameof(PairEntity.TokenId)},
                {nameof(PairEntity.ReserveCrs)},
                {nameof(PairEntity.ReserveSrc)},
                {nameof(PairEntity.CreatedDate)}
            FROM pair;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public SelectAllPairsQueryHandler(IDbContext context, IMapper mapper, 
            ILogger<SelectAllPairsQueryHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<IEnumerable<Pair>> Handle(SelectAllPairsQuery request, CancellationToken cancellationToken)
        {
            var command = DatabaseQuery.Create(SqlCommand, null, cancellationToken);
            
            var tokenEntities =  await _context.ExecuteQueryAsync<PairEntity>(command);

            return _mapper.Map<IEnumerable<Pair>>(tokenEntities);
        }
    }
}