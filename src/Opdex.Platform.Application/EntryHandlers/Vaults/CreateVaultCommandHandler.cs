using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults
{
    public class CreateVaultCommandHandler : IRequestHandler<CreateVaultCommand, long>
    {
        private readonly IMediator _mediator;

        public CreateVaultCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<long> Handle(CreateVaultCommand request, CancellationToken cancellationToken)
        {
            var vault = await _mediator.Send(new RetrieveVaultByAddressQuery(request.Vault, findOrThrow: request.IsUpdate));

            var hasApplicableUpdates = request.IsUpdate && vault.ModifiedBlock < request.BlockHeight;

            if (vault != null && !hasApplicableUpdates)
            {
                return vault.Id;
            }

            var summary = await _mediator.Send(new RetrieveVaultContractSummaryCommand(request.Vault, request.BlockHeight));

            if (hasApplicableUpdates)
            {
                // vault.Update(summary, request.BlockHeight);
            }
            else
            {
                var token = await _mediator.Send(new RetrieveTokenByAddressQuery(summary.LockedToken));

                vault = new Vault(request.Vault, token.Id, summary.Owner, summary.Genesis, summary.UnassignedSupply, request.BlockHeight);
            }

            // Todo: What if "Make" commands took "isUpdate" for both updates and rewinds, they're the same right?? Removes duplicate logic
            return await _mediator.Send(new MakeVaultCommand(vault, request.BlockHeight, rewind: false));
        }
    }
}
