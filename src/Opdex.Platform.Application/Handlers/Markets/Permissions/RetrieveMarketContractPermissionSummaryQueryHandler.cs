using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Markets.Permissions;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Markets;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Markets.Permissions;

public class RetrieveMarketContractPermissionSummaryQueryHandler
    : IRequestHandler<RetrieveMarketContractPermissionSummaryQuery, MarketContractPermissionSummary>
{
    private readonly IMediator _mediator;

    public RetrieveMarketContractPermissionSummaryQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<MarketContractPermissionSummary> Handle(RetrieveMarketContractPermissionSummaryQuery request, CancellationToken cancellationToken)
    {
        var summary = new MarketContractPermissionSummary(request.BlockHeight);

        if (request.IncludeAuthorization)
        {
            var authorization = await _mediator.Send(new CallCirrusGetMarketPermissionAuthorizationQuery(request.Market,
                                                                                                         request.Wallet,
                                                                                                         request.PermissionType,
                                                                                                         request.BlockHeight));

            summary.SetAuthorization(authorization);
        }

        // Todo: This should have IncludeBlame however to retrieve this value, we have to search transaction receipts and logs.

        return summary;
    }
}