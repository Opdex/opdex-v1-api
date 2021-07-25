using MediatR;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.Tokens;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.Handlers.Addresses
{
    public class RetrieveAddressAllowanceQueryHandler : IRequestHandler<RetrieveAddressAllowanceQuery, AddressAllowance>
    {
        private readonly IMediator _mediator;

        public RetrieveAddressAllowanceQueryHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<AddressAllowance> Handle(RetrieveAddressAllowanceQuery request, CancellationToken cancellationToken)
        {
            var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Token), cancellationToken);

            var allowanceResponse = await _mediator.Send(new CallCirrusGetSrcTokenAllowanceQuery(request.Token, request.Owner, request.Spender), cancellationToken);

            return new AddressAllowance(token.Id, request.Owner, request.Spender, allowanceResponse, 1);
        }
    }
}
