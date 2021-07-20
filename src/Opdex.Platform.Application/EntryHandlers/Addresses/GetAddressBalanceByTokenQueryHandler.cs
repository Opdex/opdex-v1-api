using MediatR;
using Opdex.Platform.Application.Abstractions.EntryQueries.Addresses;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Addresses;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Addresses
{
    public class GetAddressBalanceByTokenQueryHandler : IRequestHandler<GetAddressBalanceByTokenQuery, AddressBalanceDto>
    {
        private readonly IMediator _mediator;
        private readonly IModelAssembler<AddressBalance, AddressBalanceDto> _assembler;

        public GetAddressBalanceByTokenQueryHandler(IMediator mediator, IModelAssembler<AddressBalance, AddressBalanceDto> assembler)
        {
            _mediator = mediator;
            _assembler = assembler;
        }

        public async Task<AddressBalanceDto> Handle(GetAddressBalanceByTokenQuery request, CancellationToken cancellationToken)
        {
            var addressBalance = await _mediator.Send(new RetrieveAddressBalanceByTokenAddressAndOwnerQuery(request.TokenAddress, request.Address), cancellationToken);
            return await _assembler.Assemble(addressBalance);
        }
    }
}
