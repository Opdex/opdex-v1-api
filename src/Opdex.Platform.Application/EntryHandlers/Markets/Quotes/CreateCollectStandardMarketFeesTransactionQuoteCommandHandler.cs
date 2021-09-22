using MediatR;
using Opdex.Platform.Application.Abstractions.EntryCommands.Markets.Quotes;
using Opdex.Platform.Application.Abstractions.Models.Transactions;
using Opdex.Platform.Application.Abstractions.Queries.Markets;
using Opdex.Platform.Application.Abstractions.Queries.Tokens;
using Opdex.Platform.Application.Assemblers;
using Opdex.Platform.Application.EntryHandlers.Transactions;
using Opdex.Platform.Common.Configurations;
using Opdex.Platform.Common.Constants.SmartContracts.Markets;
using Opdex.Platform.Common.Exceptions;
using Opdex.Platform.Common.Extensions;
using Opdex.Platform.Common.Models;
using Opdex.Platform.Domain.Models.Transactions;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Opdex.Platform.Application.EntryHandlers.Markets.Quotes
{
    public class CreateCollectStandardMarketFeesTransactionQuoteCommandHandler : BaseTransactionQuoteCommandHandler<CreateCollectStandardMarketFeesTransactionQuoteCommand>
    {
        private const string MethodName = StandardMarketConstants.Methods.CollectMarketFees;
        private readonly FixedDecimal CrsToSend = FixedDecimal.Zero;

        public CreateCollectStandardMarketFeesTransactionQuoteCommandHandler(IModelAssembler<TransactionQuote, TransactionQuoteDto> quoteAssembler,
                                                                             IMediator mediator, OpdexConfiguration config) : base(quoteAssembler, mediator, config)
        {
        }

        public override async Task<TransactionQuoteDto> Handle(CreateCollectStandardMarketFeesTransactionQuoteCommand request, CancellationToken cancellationToken)
        {
            // ensure market exists, if not throw to return 404
            _ = await _mediator.Send(new RetrieveMarketByAddressQuery(request.Market, findOrThrow: true), cancellationToken);

            var token = await _mediator.Send(new RetrieveTokenByAddressQuery(request.Token, findOrThrow: false), cancellationToken);
            if (token is null) throw new InvalidDataException(nameof(request.Token), "Token address is not known.");

            var amount = request.Amount.ToSatoshis(token.Decimals);

            var requestParameters = new List<TransactionQuoteRequestParameter>
            {
                new TransactionQuoteRequestParameter("Token", new SmartContractMethodParameter(request.Token)),
                new TransactionQuoteRequestParameter("Amount", new SmartContractMethodParameter(amount))
            };

            var quoteRequest = new TransactionQuoteRequest(request.WalletAddress, request.Market, CrsToSend, MethodName, _callbackEndpoint, requestParameters);

            return await ExecuteAsync(quoteRequest, cancellationToken);
        }
    }
}
