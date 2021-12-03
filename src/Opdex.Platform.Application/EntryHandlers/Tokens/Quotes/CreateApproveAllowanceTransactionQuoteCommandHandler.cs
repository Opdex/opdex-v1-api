using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Addresses.Allowances;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts.Tokens;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Tokens.Quotes;

public class CreateApproveAllowanceTransactionQuoteCommandHandler
    : BaseTransactionQuoteCommandHandler<CreateApproveAllowanceTransactionQuoteCommand>
{
    const string MethodName = StandardTokenConstants.Methods.Approve;
    private static readonly FixedDecimal AmountCrs = FixedDecimal.Zero;

    public CreateApproveAllowanceTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                                IMediator mediator, OpdexConfiguration config)
        : base(quoteAssembler, mediator, config)
    {
    }

    public override async Task<TransactionQuoteDto> Handle(CreateApproveAllowanceTransactionQuoteCommand request, CancellationToken cancellationToken)
    {
        // ensure mining governance contract exists, throw 404 if not
        var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Token), cancellationToken);

        var currentAllowance = await _mediator.Send(new RetrieveAddressAllowanceQuery(request.WalletAddress, request.Spender, request.Token), cancellationToken);
        var newAmount = request.Amount.ToSatoshis(token.Decimals);

        var requestParameters = new List<TransactionQuoteRequestParameter>
        {
            new TransactionQuoteRequestParameter("Spender", request.Spender),
            new TransactionQuoteRequestParameter("Current Allowance", currentAllowance.Allowance),
            new TransactionQuoteRequestParameter("New Allowance", newAmount)
        };

        var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, request.Token, AmountCrs, MethodName, _callbackEndpoint, requestParameters);

        return await ExecuteAsync(quoteRequest, cancellationToken);
    }
}