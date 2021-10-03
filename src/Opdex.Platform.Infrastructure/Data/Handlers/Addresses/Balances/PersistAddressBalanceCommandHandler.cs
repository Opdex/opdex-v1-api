using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.Addresses;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Addresses.Balances
{
    public class PersistAddressBalanceCommandHandler : IRequestHandler<PersistAddressBalanceCommand, ulong>
    {
        private static readonly string InsertSqlCommand =
            $@"INSERT INTO address_balance (
                {nameof(AddressBalanceEntity.Id)},
                {nameof(AddressBalanceEntity.TokenId)},
                {nameof(AddressBalanceEntity.Owner)},
                {nameof(AddressBalanceEntity.Balance)},
                {nameof(AddressBalanceEntity.CreatedBlock)},
                {nameof(AddressBalanceEntity.ModifiedBlock)}
              ) VALUES (
                @{nameof(AddressBalanceEntity.Id)},
                @{nameof(AddressBalanceEntity.TokenId)},
                @{nameof(AddressBalanceEntity.Owner)},
                @{nameof(AddressBalanceEntity.Balance)},
                @{nameof(AddressBalanceEntity.CreatedBlock)},
                @{nameof(AddressBalanceEntity.ModifiedBlock)}
              );
              SELECT LAST_INSERT_ID()";

        private static readonly string UpdateSqlCommand =
            $@"UPDATE address_balance
                SET
                    {nameof(AddressBalanceEntity.Balance)} = @{nameof(AddressBalanceEntity.Balance)},
                    {nameof(AddressBalanceEntity.ModifiedBlock)} = @{nameof(AddressBalanceEntity.ModifiedBlock)}
                WHERE {nameof(AddressBalanceEntity.Id)} = @{nameof(AddressBalanceEntity.Id)};";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public PersistAddressBalanceCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistAddressBalanceCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ulong> Handle(PersistAddressBalanceCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<AddressBalanceEntity>(request.AddressBalance);

                var isUpdate = entity.Id >= 1;

                var sql = isUpdate ? UpdateSqlCommand : InsertSqlCommand;

                var command = DatabaseQuery.Create(sql, entity, cancellationToken);

                var result = await _context.ExecuteScalarAsync<ulong>(command);

                return isUpdate ? entity.Id : result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failure persisting address balance for owner: {request.AddressBalance.Owner}");

                return 0;
            }
        }
    }
}
