using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vault;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models.ODX;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Vault
{
    public class PersistVaultCommandHandler : IRequestHandler<PersistVaultCommand, long>
    {
        private static readonly string SqlCommand =
            $@"INSERT INTO odx_vault (
                {nameof(VaultEntity.Address)},
                {nameof(VaultEntity.TokenId)},
                {nameof(VaultEntity.Owner)}
              ) VALUES (
                @{nameof(VaultEntity.Address)},
                @{nameof(VaultEntity.TokenId)},
                @{nameof(VaultEntity.Owner)}
              );
              SELECT LAST_INSERT_ID()";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistVaultCommandHandler(IDbContext context, IMapper mapper, 
            ILogger<PersistVaultCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<long> Handle(PersistVaultCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = _mapper.Map<VaultEntity>(request.Vault);
                var command = DatabaseQuery.Create(SqlCommand, entity, cancellationToken);
                return await _context.ExecuteScalarAsync<long>(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failure persisting vault.");
                return 0;
            }
        }
    }
}