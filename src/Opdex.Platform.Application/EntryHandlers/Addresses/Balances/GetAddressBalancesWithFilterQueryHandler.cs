using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Balances;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Tokens;
using Opdex.Platform.Domain.Models.Addresses;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Addresses.Balances;

public class GetAddressBalancesWithFilterQueryHandler : EntryFilterQueryHandler<GetAddressBalancesWithFilterQuery, AddressBalancesDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<AddressBalance, AddressBalanceDto> _assembler;
    private readonly ILogger<GetAddressBalancesWithFilterQueryHandler> _logger;

    public GetAddressBalancesWithFilterQueryHandler(IMediator mediator, IModelAssembler<AddressBalance, AddressBalanceDto> assembler, ILogger<GetAddressBalancesWithFilterQueryHandler> logger)
        : base(logger)
    {
        _mediator = mediator;
        _assembler = assembler ?? throw new ArgumentNullException(nameof(assembler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public override async Task<AddressBalancesDto> Handle(GetAddressBalancesWithFilterQuery request, CancellationToken cancellationToken)
    {
        var balances = await _mediator.Send(new RetrieveAddressBalancesWithFilterQuery(request.Address, request.Cursor), cancellationToken);

        var balancesResults = balances.ToList();

        _logger.LogTrace("Retrieved queried address balances");

        var cursorDto = BuildCursorDto(balancesResults, request.Cursor, pointerSelector: result => result.Id);

        _logger.LogTrace("Returning {ResultCount} results", balancesResults.Count);

        var dtos = await Task.WhenAll(balancesResults.Select(balance => _assembler.Assemble(balance)));

        _logger.LogTrace("Assembled results");

        return new AddressBalancesDto { Balances = dtos, Cursor = cursorDto };
    }
}
