using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults;

public class MakeVaultCommandHandler : IRequestHandler<MakeVaultCommand, ulong>
{
    private readonly IMediator _mediator;

    public MakeVaultCommandHandler(IMediator mediator)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
    }

    public async Task<ulong> Handle(MakeVaultCommand request, CancellationToken cancellationToken)
    {
        if (request.Refresh)
        {
            var summary = await _mediator.Send(new RetrieveVaultContractSummaryQuery(request.Vault.Address, request.BlockHeight,
                                                                                               includeUnassignedSupply: request.RefreshUnassignedSupply,
                                                                                               includeProposedSupply: request.RefreshProposedSupply,
                                                                                               includeTotalPledgeMinimum: request.RefreshTotalPledgeMinimum,
                                                                                               includeTotalVoteMinimum: request.RefreshTotalVoteMinimum), CancellationToken.None);

            request.Vault.Update(summary);
        }

        return await _mediator.Send(new PersistVaultCommand(request.Vault), CancellationToken.None);
    }
}
