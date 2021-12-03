using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Allowances;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Allowances;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Addresses;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Addresses.Allowances;

public class GetAddressAllowanceQueryHandler : IRequestHandler<GetAddressAllowanceQuery, AddressAllowanceDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<AddressAllowance, AddressAllowanceDto> _assembler;

    public GetAddressAllowanceQueryHandler(IMediator mediator, IModelAssembler<AddressAllowance, AddressAllowanceDto> assembler)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _assembler = assembler ?? throw new ArgumentNullException(nameof(assembler));
    }

    public async Task<AddressAllowanceDto> Handle(GetAddressAllowanceQuery request, CancellationToken cancellationToken)
    {
        var allowance = await _mediator.Send(new RetrieveAddressAllowanceQuery(request.Owner, request.Spender, request.Token), cancellationToken);

        return await _assembler.Assemble(allowance);
    }
}