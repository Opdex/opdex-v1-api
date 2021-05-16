using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Addresses
{
    public class PersistAddressMiningCommandHandler : IRequestHandler<PersistAddressMiningCommand, long>
    {
        private static readonly string InsertSqlCommand =
            $@"INSERT INTO address_mining (
                {nameof(AddressMiningEntity.Id)},
                {nameof(AddressMiningEntity.MiningPoolId)},
                {nameof(AddressMiningEntity.Owner)},
                {nameof(AddressMiningEntity.Balance)},
                {nameof(AddressMiningEntity.CreatedBlock)},
                {nameof(AddressMiningEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(AddressMiningEntity.Id)},
                @{nameof(AddressMiningEntity.MiningPoolId)},
                @{nameof(AddressMiningEntity.Owner)},
                @{nameof(AddressMiningEntity.Balance)},
                @{nameof(AddressMiningEntity.CreatedBlock)},
                @{nameof(AddressMiningEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID()";
        
        private static readonly string UpdateSqlCommand =
            $@"UPDATE address_mining 
                SET 
                    {nameof(AddressMiningEntity.Balance)} = @{nameof(AddressMiningEntity.Balance)}
                WHERE {nameof(AddressMiningEntity.Id)} = @{nameof(AddressMiningEntity.Id)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistAddressMiningCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistAddressMiningCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<long> Handle(PersistAddressMiningCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<AddressMiningEntity>(request.AddressMining);

                var isUpdate = entity.Id >= 1;

                var sql = isUpdate ? UpdateSqlCommand : InsertSqlCommand;
                
                var command = DatabaseQuery.Create(sql, entity, cancellationToken);
                
                var result = await _context.ExecuteScalarAsync<long>(command);
                
                return isUpdate ? entity.Id : result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure persisting address mining for owner: {request.AddressMining.Owner}");
                
                return 0;
            }
        }
    }
}