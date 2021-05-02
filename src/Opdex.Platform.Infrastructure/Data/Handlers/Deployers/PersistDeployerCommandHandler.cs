using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Deployers;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Deployers
{
    public class PersistDeployerCommandHandler : IRequestHandler<PersistDeployerCommand, long>
    {
        // Todo: Insert vs update
        private static readonly string SqlCommand =
            $@"INSERT INTO deployer (
                {nameof(DeployerEntity.Address)},
                {nameof(DeployerEntity.CreatedDate)}
              ) VALUES (
                @{nameof(DeployerEntity.Address)},
                UTC_TIMESTAMP()
              );
              SELECT LAST_INSERT_ID();";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public PersistDeployerCommandHandler(IDbContext context, IMapper mapper, ILogger<PersistDeployerCommandHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<long> Handle(PersistDeployerCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var command = DatabaseQuery.Create(SqlCommand, request.Address, cancellationToken);
            
                return await _context.ExecuteScalarAsync<long>(command);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Unable to persist deployer {request.Address}");
                
                return 0;
            }
        }
    }
}