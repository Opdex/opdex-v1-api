using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults
{
    public class GetVaultsWithFilterQueryHandler : EntryFilterQueryHandler<GetVaultsWithFilterQuery, VaultsDto>
    {
        private readonly IMediator _mediator;
        private readonly IModelAssembler<Vault, VaultDto> _vaultAssembler;

        public GetVaultsWithFilterQueryHandler(IMediator mediator, IModelAssembler<Vault, VaultDto> vaultAssembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _vaultAssembler = vaultAssembler ?? throw new ArgumentNullException(nameof(vaultAssembler));
        }

        public async override Task<VaultsDto> Handle(GetVaultsWithFilterQuery request, CancellationToken cancellationToken)
        {
            var vaults = await _mediator.Send(new RetrieveVaultsWithFilterQuery(request.Cursor), cancellationToken);

            var vaultsResults = vaults.ToList();

            var cursorDto = BuildCursorDto(vaultsResults, request.Cursor, pointerSelector: result => result.Id);

            var assembledResults = await Task.WhenAll(vaultsResults.Select(vault => _vaultAssembler.Assemble(vault)));

            return new VaultsDto { Vaults = assembledResults, Cursor = cursorDto };
        }
    }
}
