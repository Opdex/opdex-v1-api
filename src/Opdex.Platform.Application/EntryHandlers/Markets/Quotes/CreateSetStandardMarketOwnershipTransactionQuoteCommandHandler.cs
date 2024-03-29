using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts.Markets;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Markets.Quotes;

public class CreateSetStandardMarketOwnershipTransactionQuoteCommandHandler : BaseTransactionQuoteCommandHandler<CreateSetStandardMarketOwnershipTransactionQuoteCommand>
{
    private const string MethodName = StandardMarketConstants.Methods.SetPendingOwnership;
    private readonly FixedDecimal CrsToSend = FixedDecimal.Zero;

    public CreateSetStandardMarketOwnershipTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                                          IMediator mediator, OpdexConfiguration config) : base(quoteAssembler, mediator, config)
    {
    }

    public override async Task<TransactionQuoteDto> Handle(CreateSetStandardMarketOwnershipTransactionQuoteCommand request, CancellationToken cancellationToken)
    {
        // ensure market exists, if not throw to return 404
        _ = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Market, findOrThrow: true), cancellationToken);

        var requestParameters = new List<TransactionQuoteRequestParameter>
        {
            new TransactionQuoteRequestParameter("New Owner", new SmartContractMethodParameter(request.NewOwner))
        };

        var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, request.Market, CrsToSend, MethodName, _callbackEndpoint, requestParameters);

        return await ExecuteAsync(quoteRequest, cancellationToken);
    }
}
