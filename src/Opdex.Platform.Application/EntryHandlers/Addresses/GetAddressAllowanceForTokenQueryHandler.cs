using AutoMapper;
using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Addresses
{
    public class GetAddressAllowanceForTokenQueryHandler : IRequestHandler<GetAddressAllowanceForTokenQuery, AddressAllowanceDto>
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public GetAddressAllowanceForTokenQueryHandler(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<AddressAllowanceDto> Handle(GetAddressAllowanceForTokenQuery request, CancellationToken cancellationToken)
        {
            var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Token, findOrThrow: true), cancellationToken);

            var addressAllowance = await _mediator.Send(new RetrieveAddressAllowanceByTokenIdAndOwnerAndSpenderQuery(token.Id, request.Owner, request.Spender, findOrThrow: true), cancellationToken);

            return _mapper.Map<AddressAllowanceDto>(addressAllowance);
        }
    }
}
