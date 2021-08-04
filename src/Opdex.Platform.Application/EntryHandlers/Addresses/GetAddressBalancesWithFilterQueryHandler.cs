using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Addresses;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Addresses
{
    public class GetAddressBalancesWithFilterQueryHandler : EntryFilterQueryHandler<GetAddressBalancesWithFilterQuery, AddressBalancesDto>
    {
        private readonly IMediator _mediator;
        private readonly IModelAssembler<AddressBalance, AddressBalanceDto> _assembler;

        public GetAddressBalancesWithFilterQueryHandler(IMediator mediator, IModelAssembler<AddressBalance, AddressBalanceDto> assembler)
        {
            _mediator = mediator;
            _assembler = assembler ?? throw new ArgumentNullException(nameof(assembler));
        }

        public override async Task<AddressBalancesDto> Handle(GetAddressBalancesWithFilterQuery request, CancellationToken cancellationToken)
        {
            var balances = await _mediator.Send(new RetrieveAddressBalancesWithFilterQuery(request.Address, request.Cursor), cancellationToken);

            var balancesResults = balances.ToList();

            var cursorDto = BuildCursorDto(balancesResults, request.Cursor, pointerSelector: result => result.Id);

            var dtos = await Task.WhenAll(balancesResults.Select(balance => _assembler.Assemble(balance)));

            return new AddressBalancesDto { Balances = dtos, Cursor = cursorDto };
        }
    }
}
