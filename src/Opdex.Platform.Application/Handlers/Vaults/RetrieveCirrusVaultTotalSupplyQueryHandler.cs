using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults
{
    public class RetrieveCirrusVaultTotalSupplyQueryHandler : IRequestHandler<RetrieveCirrusVaultTotalSupplyQuery, string>
    {
        private readonly IMediator _mediator;

        public RetrieveCirrusVaultTotalSupplyQueryHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public Task<string> Handle(RetrieveCirrusVaultTotalSupplyQuery request, CancellationToken cancellationToken)
        {
            return _mediator.Send(new CallCirrusGetVaultTotalSupplyQuery(request.VaultAddress, request.BlockHeight), cancellationToken);
        }
    }
}
