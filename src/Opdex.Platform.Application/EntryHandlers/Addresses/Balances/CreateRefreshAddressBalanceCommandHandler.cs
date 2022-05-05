using MediatR;
using Microsoft.Extensions.Logging;
using Opdex.Platform.Application.Abstractions.EntryCommands.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Models.Addresses;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Balances;
using Opdex.Platform.Application.Abstractions.Queries.Blocks;
using Opdex.Platform.Application.Abstractions.Queries.LiquidityPools;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Domain.Models.Addresses;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Addresses.Balances;

public class CreateRefreshAddressBalanceCommandHandler : IRequestHandler<CreateRefreshAddressBalanceCommand, AddressBalanceDto>
{
    private readonly IMediator _mediator;
    private readonly IModelAssembler<AddressBalance, AddressBalanceDto> _addressBalanceAssembler;
    private readonly ILogger<CreateRefreshAddressBalanceCommandHandler> _logger;

    public CreateRefreshAddressBalanceCommandHandler(IMediator mediator, IModelAssembler<AddressBalance, AddressBalanceDto> addressBalanceAssembler,
        ILogger<CreateRefreshAddressBalanceCommandHandler> logger)
    {
        _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        _addressBalanceAssembler = addressBalanceAssembler ?? throw new ArgumentNullException(nameof(addressBalanceAssembler));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<AddressBalanceDto> Handle(CreateRefreshAddressBalanceCommand request, CancellationToken cancellationToken)
    {
        var block = await _mediator.Send(new RetrieveLatestBlockQuery(), cancellationToken);

        await _mediator.Send(new CreateAddressBalanceCommand(request.Wallet, request.Token, block.Height), cancellationToken);
        var balance = await _mediator.Send(new RetrieveAddressBalanceByOwnerAndTokenQuery(request.Wallet, tokenAddress: request.Token), cancellationToken);

        var attributes = await _mediator.Send(new RetrieveTokenAttributesByTokenAddressQuery(request.Token), cancellationToken);
        if (attributes.Select(a => a.AttributeType).All(a => a != TokenAttributeType.Provisional))
        {
            return await _addressBalanceAssembler.Assemble(balance);
        }

        // if token is OLPT, refresh the SRC balance of the pool token
        var pool = await _mediator.Send(new RetrieveLiquidityPoolByAddressQuery(request.Token, false), cancellationToken);
        if (pool is null) _logger.LogError("Unable to find pool for OLPT with address {Token}", request.Token);
        var srcToken = await _mediator.Send(new RetrieveTokenByIdQuery(pool!.SrcTokenId, false), cancellationToken);
        if (srcToken is null) _logger.LogError("Unable to find SRC token of pool with address {Pool}", pool.Address);

        await _mediator.Send(new CreateAddressBalanceCommand(request.Wallet, srcToken!.Address, block.Height), cancellationToken);

        return await _addressBalanceAssembler.Assemble(balance);
    }
}
