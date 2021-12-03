using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Deployers;
using Opdex.Platform.Common.Constants.SmartContracts;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Deployers;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Deployers;

public class RetrieveDeployerContractSummaryQueryHandler : IRequestHandler<RetrieveDeployerContractSummaryQuery, DeployerContractSummary>
{
    private readonly IMediator _mediator;

    public RetrieveDeployerContractSummaryQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<DeployerContractSummary> Handle(RetrieveDeployerContractSummaryQuery request, CancellationToken cancellationToken)
    {
        var summary = new DeployerContractSummary(request.BlockHeight);

        if (request.IncludePendingOwner)
        {
            var pendingOwner = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Deployer,
                                                                                                MarketDeployerConstants.StateKeys.PendingOwner,
                                                                                                SmartContractParameterType.Address,
                                                                                                request.BlockHeight));

            summary.SetPendingOwner(pendingOwner);
        }

        if (request.IncludeOwner)
        {
            var owner = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Deployer,
                                                                                         MarketDeployerConstants.StateKeys.Owner,
                                                                                         SmartContractParameterType.Address,
                                                                                         request.BlockHeight));

            summary.SetOwner(owner);
        }

        return summary;
    }
}