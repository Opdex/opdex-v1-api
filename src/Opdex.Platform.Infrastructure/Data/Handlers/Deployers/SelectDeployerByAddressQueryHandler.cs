using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Deployers;
using Opdex.Platform.Infrastructure.Abstractions.Data;
using Opdex.Platform.Infrastructure.Abstractions.Data.Models;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Deployers;

namespace Opdex.Platform.Infrastructure.Data.Handlers.Deployers;

public class SelectDeployerByAddressQueryHandler : IRequestHandler<SelectDeployerByAddressQuery, Deployer>
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
            WHERE {nameof(DeployerEntity.Address)} = @{nameof(SqlParams.Address)}
            LIMIT 1;";

    private readonly IDbContext _context;
    private readonly IMapper _mapper;

    public SelectDeployerByAddressQueryHandler(IDbContext context, IMapper mapper)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Deployer> Handle(SelectDeployerByAddressQuery request, CancellationToken cancellationToken)
    {
        var sqlParams = new SqlParams(request.Address);

        var command = DatabaseQuery.Create(SqlCommand, sqlParams, cancellationToken);

        var result = await _context.ExecuteFindAsync<DeployerEntity>(command);

        if (request.FindOrThrow && result == null)
        {
            throw new NotFoundException($"{nameof(Deployer)} not found.");
        }

        return result == null ? null : _mapper.Map<Deployer>(result);
    }

    private sealed class SqlParams
    {
        internal SqlParams(Address address)
        {
            Address = address;
        }

        public Address Address { get; }
    }
}