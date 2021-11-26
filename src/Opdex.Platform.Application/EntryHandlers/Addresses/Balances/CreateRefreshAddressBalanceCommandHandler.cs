using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Domain.Models.Addresses;
using Opdex.Platform.Infrastructure.Abstractions.Clients.CirrusFullNodeApi.Queries.BlockStore;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Addresses.Balances
{
    public class CreateRefreshAddressBalanceCommandHandler : IRequestHandler<CreateRefreshAddressBalanceCommand, AddressBalanceDto>
    {
        private readonly IMediator _mediator;
        private readonly IModelAssembler<AddressBalance, AddressBalanceDto> _addressBalanceAssembler;

        public CreateRefreshAddressBalanceCommandHandler(IMediator mediator, IModelAssembler<AddressBalance, AddressBalanceDto> addressBalanceAssembler)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _addressBalanceAssembler = addressBalanceAssembler ?? throw new ArgumentNullException(nameof(addressBalanceAssembler));
        }

        public async Task<AddressBalanceDto> Handle(CreateRefreshAddressBalanceCommand request, CancellationToken cancellationToken)
        {
            var block = await _mediator.Send(new RetrieveLatestBlockQuery(), cancellationToken);

            await _mediator.Send(new CreateAddressBalanceCommand(request.Wallet, request.Token, block.Height), cancellationToken);
            var balance = await _mediator.Send(new RetrieveAddressBalanceByOwnerAndTokenQuery(request.Wallet, tokenAddress: request.Token), cancellationToken);

            return await _addressBalanceAssembler.Assemble(balance);
        }
    }
}