using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Common.Constants.SmartContracts.Markets;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.SmartContracts;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Markets;

public class RetrieveMarketContractSummaryQueryHandler
    : IRequestHandler<RetrieveMarketContractSummaryQuery, MarketContractSummary>
{
    private readonly IMediator _mediator;

    public RetrieveMarketContractSummaryQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<MarketContractSummary> Handle(RetrieveMarketContractSummaryQuery request, CancellationToken cancellationToken)
    {
        var summary = new MarketContractSummary(request.BlockHeight);

        if (request.IncludePendingOwner)
        {
            var pendingOwner = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Market,
                                                                                                StandardMarketConstants.StateKeys.PendingOwner,
                                                                                                SmartContractParameterType.Address,
                                                                                                request.BlockHeight));

            summary.SetPendingOwner(pendingOwner);
        }

        if (request.IncludeOwner)
        {
            var owner = await _mediator.Send(new CallCirrusGetSmartContractPropertyQuery(request.Market,
                                                                                         StandardMarketConstants.StateKeys.Owner,
                                                                                         SmartContractParameterType.Address,
                                                                                         request.BlockHeight));

            summary.SetOwner(owner);
        }

        return summary;
    }
}