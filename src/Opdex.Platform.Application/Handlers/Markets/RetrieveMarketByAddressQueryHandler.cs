using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Domain.Models.Markets;
using Opdex.Platform.Infrastructure.Abstractions.Data.Queries.Markets;

namespace Opdex.Platform.Application.Handlers.Markets;

public class RetrieveMarketByAddressQueryHandler : IRequestHandler<RetrieveMarketByAddressQuery, Market>
{
    private readonly IMediator _mediator;

    public RetrieveMarketByAddressQueryHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public Task<Market> Handle(RetrieveMarketByAddressQuery request, CancellationToken cancellationToken)
    {
        return _mediator.Send(new SelectMarketByAddressQuery(request.Address, request.FindOrThrow), cancellationToken);
    }
}