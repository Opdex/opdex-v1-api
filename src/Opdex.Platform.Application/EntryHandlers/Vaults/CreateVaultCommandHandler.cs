using MediatR;
using Opdex.Platform.Application.Abstractions.Commands.Vaults;
using Opdex.Platform.Application.Abstractions.EntryCommands.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults
{
    public class CreateVaultCommandHandler : IRequestHandler<CreateVaultCommand, ulong>
    {
        private readonly IMediator _mediator;

        public CreateVaultCommandHandler(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        public async Task<ulong> Handle(CreateVaultCommand request, CancellationToken cancellationToken)
        {
            var vault = await _mediator.Send(new RetrieveVaultByAddressQuery(request.Vault, findOrThrow: false));

            if (vault != null) return vault.Id;

            vault = new Vault(request.Vault, request.TokenId, request.Owner, request.BlockHeight);

            return await _mediator.Send(new MakeVaultCommand(vault, request.BlockHeight));
        }
    }
}
