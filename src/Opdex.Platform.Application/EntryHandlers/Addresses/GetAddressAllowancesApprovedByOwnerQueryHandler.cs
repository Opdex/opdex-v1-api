using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Domain.Models.Addresses;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Addresses
{
    public class GetAddressAllowancesApprovedByOwnerQueryHandler : IRequestHandler<GetAddressAllowancesApprovedByOwnerQuery, IEnumerable<AddressAllowanceDto>>
    {
        private readonly IModelAssembler<AddressAllowance, AddressAllowanceDto> _assembler;
        private readonly IMediator _mediator;

        public GetAddressAllowancesApprovedByOwnerQueryHandler(IModelAssembler<AddressAllowance, AddressAllowanceDto> assembler, IMediator mediator)
        {
            _assembler = assembler;
            _mediator = mediator;
        }

        public async Task<IEnumerable<AddressAllowanceDto>> Handle(GetAddressAllowancesApprovedByOwnerQuery request, CancellationToken cancellationToken)
        {
            var tokenId = 0L;

            if (request.Token.HasValue())
            {
                var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Token, findOrThrow: true), cancellationToken);
                tokenId = token.Id;
            }

            var addressAllowances = await _mediator.Send(new RetrieveAddressAllowancesByOwnerWithFilterQuery(request.Owner, request.Spender, tokenId), cancellationToken);
            return await Task.WhenAll(addressAllowances.Select(addressAllowance => _assembler.Assemble(addressAllowance)));
        }
    }
}
