using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Domain.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Deployers;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Deployers
{
    public class SelectDeployerByAddressQueryHandler : IRequestHandler<SelectDeployerByAddressQuery, Deployer>
    {
        private static readonly string SqlCommand =
            $@"SELECT
                {nameof(DeployerEntity.Id)},
                {nameof(DeployerEntity.Address)},
                {nameof(DeployerEntity.CreatedDate)}
            FROM deployer
            WHERE {nameof(DeployerEntity.Address)} = @{nameof(DeployerEntity.Address)}
            LIMIT 1;";

        private readonly IDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        
        public SelectDeployerByAddressQueryHandler(IDbContext context, IMapper mapper, 
            ILogger<SelectDeployerByAddressQueryHandler> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        
        public async Task<Deployer> Handle(SelectDeployerByAddressQuery request, CancellationToken cancellationToken)
        {
            var command = DatabaseQuery.Create(SqlCommand, request, cancellationToken);
            
            var entity =  await _context.ExecuteFindAsync<DeployerEntity>(command);

            if (entity == null)
            {
                throw new NotFoundException($"{nameof(Deployer)} not found with address {request.Address}");
            }

            return _mapper.Map<Deployer>(entity);
        }
    }
}