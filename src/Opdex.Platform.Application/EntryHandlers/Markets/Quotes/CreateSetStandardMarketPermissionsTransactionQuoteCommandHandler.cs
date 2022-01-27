using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts.Markets;
using Opdex.Platform.Common.Enums;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Markets.Quotes;

public class CreateSetStandardMarketPermissionsTransactionQuoteCommandHandler : BaseTransactionQuoteCommandHandler<CreateSetStandardMarketPermissionsTransactionQuoteCommand>
{
    private const string MethodName = StandardMarketConstants.Methods.Authorize;
    private readonly FixedDecimal CrsToSend = FixedDecimal.Zero;

    public CreateSetStandardMarketPermissionsTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                                            IMediator mediator, OpdexConfiguration config) : base(quoteAssembler, mediator, config)
    {
    }

    public override async Task<TransactionQuoteDto> Handle(CreateSetStandardMarketPermissionsTransactionQuoteCommand request, CancellationToken cancellationToken)
    {
        var market = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Market, findOrThrow: true), cancellationToken);
        if (market.IsStakingMarket) throw new InvalidDataException("marketAddress", "Market address must represent a standard market.");

        if (!market.AuthPoolCreators && request.Permission == MarketPermissionType.CreatePool)
            throw new InvalidDataException("Permission", "Market does not enforce authorization for pool creation.");
        if (!market.AuthProviders && request.Permission == MarketPermissionType.Provide)
            throw new InvalidDataException("Permission", "Market does not enforce authorization for providing liquidity.");
        if (!market.AuthTraders && request.Permission == MarketPermissionType.Trade)
            throw new InvalidDataException("Permission", "Market does not enforce authorization for trading.");

        var requestParameters = new List<TransactionQuoteRequestParameter>
        {
            new TransactionQuoteRequestParameter("User", new SmartContractMethodParameter(request.User)),
            new TransactionQuoteRequestParameter("Permission", new SmartContractMethodParameter((byte)request.Permission)),
            new TransactionQuoteRequestParameter("Authorize", new SmartContractMethodParameter(request.Authorize))
        };

        var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, request.Market, CrsToSend, MethodName, _callbackEndpoint, requestParameters);

        return await ExecuteAsync(quoteRequest, cancellationToken);
    }
}
