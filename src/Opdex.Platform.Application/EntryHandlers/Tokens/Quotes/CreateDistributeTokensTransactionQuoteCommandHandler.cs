using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.Tokens.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts.Tokens;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Tokens.Quotes;

public class CreateDistributeTokensTransactionQuoteCommandHandler
    : BaseTransactionQuoteCommandHandler<CreateDistributeTokensTransactionQuoteCommand>
{
    private const string MethodName = StakingTokenConstants.Methods.Distribute;
    private static readonly FixedDecimal AmountCrs = FixedDecimal.Zero;

    public CreateDistributeTokensTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                                IMediator mediator, OpdexConfiguration config)
        : base(quoteAssembler, mediator, config)
    {
    }

    public override async Task<TransactionQuoteDto> Handle(CreateDistributeTokensTransactionQuoteCommand request, CancellationToken cancellationToken)
    {
        // ensure token contract exists, throw 404 if not
        _ = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Token), cancellationToken);

        var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, request.Token, AmountCrs, MethodName, _callbackEndpoint);

        return await ExecuteAsync(quoteRequest, cancellationToken);
    }
}