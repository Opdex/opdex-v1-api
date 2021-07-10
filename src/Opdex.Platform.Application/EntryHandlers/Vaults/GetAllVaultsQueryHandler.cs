using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.ODX;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults
{
    public class GetAllVaultsQueryHandler : IRequestHandler<GetAllVaultsQuery, IEnumerable<VaultDto>>
    {
        private readonly IMediator _mediator;
        private readonly IModelAssembler<Vault, VaultDto> _vaultAssembler;

        public GetAllVaultsQueryHandler(IMediator mediator, IModelAssembler<Vault, VaultDto> vaultAssembler)
        {
            _mediator = mediator;
            _vaultAssembler = vaultAssembler;
        }

        public async Task<IEnumerable<VaultDto>> Handle(GetAllVaultsQuery request, CancellationToken cancellationToken)
        {
            var vaultDto = await _mediator.Send(new RetrieveVaultQuery(findOrThrow: false), cancellationToken);
            if (vaultDto is null)
            {
                return Enumerable.Empty<VaultDto>();
            }

            var vault = await _vaultAssembler.Assemble(vaultDto);
            return new VaultDto[] { vault };
        }
    }
}
