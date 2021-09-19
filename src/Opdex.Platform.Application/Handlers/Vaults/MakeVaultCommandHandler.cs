using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Infrastructure.Abstractions.Data.Commands.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Vaults

{
    public class MakeVaultCommandHandler : IRequestHandler<MakeVaultCommand, long>
    {
        private readonly IMediator _mediator;

        public MakeVaultCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<long> Handle(MakeVaultCommand request, CancellationToken cancellationToken)
        {
            if (request.Rewind)
            {
                var summary = await _mediator.Send(new RetrieveVaultContractSummaryCommand(request.Vault.Address, request.BlockHeight));

                // update vault
                // request.Vault.Update(summary, request.BlockHeight);
            }

            return await _mediator.Send(new PersistVaultCommand(request.Vault), cancellationToken);
        }
    }
}
