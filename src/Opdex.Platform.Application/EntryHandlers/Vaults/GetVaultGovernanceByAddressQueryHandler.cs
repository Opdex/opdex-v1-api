using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Vaults;
using Opdex.Platform.Application.Abstractions.Models.Vaults;
using Opdex.Platform.Application.Abstractions.Queries.Vaults;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Vaults;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Vaults;

public class GetVaultByAddressQueryHandler : IRequestHandler<GetVaultByAddressQuery, VaultDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<Vault, VaultDto> _vaultAssembler;

    public GetVaultByAddressQueryHandler(IMediator mediator, IModelAssembler<Vault, VaultDto> vaultAssembler)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _vaultAssembler = vaultAssembler ?? throw new ArgumentNullException(nameof(vaultAssembler));
    }

    public async Task<VaultDto> Handle(GetVaultByAddressQuery request, CancellationToken cancellationToken)
    {
        var vault = await _mediator.Send(new RetrieveVaultByAddressQuery(request.Vault, findOrThrow: true), cancellationToken);
        return await _vaultAssembler.Assemble(vault);
    }
}
