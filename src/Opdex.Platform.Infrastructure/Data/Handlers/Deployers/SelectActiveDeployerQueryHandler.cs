using AutoMapper;
using MediatR;
using Opdex.Platform.Domain.Models.Deployers;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Deployers;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Deployers;

public class SelectActiveDeployerQueryHandler : IRequestHandler<SelectActiveDeployerQuery, Deployer>
{
    private static readonly string SqlCommand =
        $@"SELECT
                {nameof(DeployerEntity.Id)},
                {nameof(DeployerEntity.Address)},
                {nameof(DeployerEntity.PendingOwner)},
                {nameof(DeployerEntity.Owner)},
                {nameof(DeployerEntity.CreatedBlock)},
                {nameof(DeployerEntity.ModifiedBlock)}
            FROM market_deployer
            LIMIT 1;";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectActiveDeployerQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    public async Task<Deployer> Handle(SelectActiveDeployerQuery request, CancellationToken cancellationToken)
    {
        var command = DatabaseQuery.Create(SqlCommand, cancellationToken);

        var result = await _context.ExecuteFindAsync<DeployerEntity>(command);

        return result is not null ? _mapper.Map<Deployer>(result) : null;
    }
}