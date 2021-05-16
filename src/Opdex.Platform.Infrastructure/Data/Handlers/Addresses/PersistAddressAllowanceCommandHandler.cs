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
    public class PersistAddressAllowanceCommandHandler : IRequestHandler<PersistAddressAllowanceCommand, long>
    {
        private static readonly string InsertSqlCommand =
            $@"INSERT INTO address_allowance (
                {nameof(AddressAllowanceEntity.Id)},
                {nameof(AddressAllowanceEntity.TokenId)},
                {nameof(AddressAllowanceEntity.LiquidityPoolId)},
                {nameof(AddressAllowanceEntity.Owner)},
                {nameof(AddressAllowanceEntity.Spender)},
                {nameof(AddressAllowanceEntity.Allowance)},
                {nameof(AddressAllowanceEntity.CreatedBlock)},
                {nameof(AddressAllowanceEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(AddressAllowanceEntity.Id)},
                @{nameof(AddressAllowanceEntity.TokenId)},
                @{nameof(AddressAllowanceEntity.LiquidityPoolId)},
                @{nameof(AddressAllowanceEntity.Owner)},
                @{nameof(AddressAllowanceEntity.Spender)},
                @{nameof(AddressAllowanceEntity.Allowance)},
                @{nameof(AddressAllowanceEntity.CreatedBlock)},
                @{nameof(AddressAllowanceEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID()";
        
        private static readonly string UpdateSqlCommand =
            $@"UPDATE address_allowance 
                SET 
                    {nameof(AddressAllowanceEntity.Allowance)} = @{nameof(AddressAllowanceEntity.Allowance)}
                WHERE {nameof(AddressAllowanceEntity.Id)} = @{nameof(AddressAllowanceEntity.Id)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistAddressAllowanceCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistAddressAllowanceCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<long> Handle(PersistAddressAllowanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<AddressAllowanceEntity>(request.AddressAllowance);

                var isUpdate = entity.Id >= 1;

                var sql = isUpdate ? UpdateSqlCommand : InsertSqlCommand;
                
                var command = DatabaseQuery.Create(sql, entity, cancellationToken);
                
                var result = await _context.ExecuteScalarAsync<long>(command);
                
                return isUpdate ? entity.Id : result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure persisting address allowance for owner: {request.AddressAllowance.Owner} and spender: {request.AddressAllowance.Spender}");
                
                return 0;
            }
        }
    }
}