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
    public class PersistAddressStakingCommandHandler : IRequestHandler<PersistAddressStakingCommand, long>
    {
        private static readonly string InsertSqlCommand =
            $@"INSERT INTO address_staking (
                {nameof(AddressStakingEntity.Id)},
                {nameof(AddressStakingEntity.LiquidityPoolId)},
                {nameof(AddressStakingEntity.Owner)},
                {nameof(AddressStakingEntity.Weight)},
                {nameof(AddressStakingEntity.CreatedBlock)},
                {nameof(AddressStakingEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(AddressStakingEntity.Id)},
                @{nameof(AddressStakingEntity.LiquidityPoolId)},
                @{nameof(AddressStakingEntity.Owner)},
                @{nameof(AddressStakingEntity.Weight)},
                @{nameof(AddressStakingEntity.CreatedBlock)},
                @{nameof(AddressStakingEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID()";
        
        private static readonly string UpdateSqlCommand =
            $@"UPDATE address_staking 
                SET 
                    {nameof(AddressStakingEntity.Weight)} = @{nameof(AddressStakingEntity.Weight)}
                WHERE {nameof(AddressStakingEntity.Id)} = @{nameof(AddressStakingEntity.Id)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistAddressStakingCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistAddressStakingCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<long> Handle(PersistAddressStakingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<AddressStakingEntity>(request.AddressStaking);

                var isUpdate = entity.Id >= 1;

                var sql = isUpdate ? UpdateSqlCommand : InsertSqlCommand;
                
                var command = DatabaseQuery.Create(sql, entity, cancellationToken);
                
                var result = await _context.ExecuteScalarAsync<long>(command);
                
                return isUpdate ? entity.Id : result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure persisting address staking for owner: {request.AddressStaking.Owner}");
                
                return 0;
            }
        }
    }
}